using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Azure;
using Microsoft.IdentityModel.Tokens;
using TastyGo.DTOs;
using TastyGo.Helpers;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.Other;
using TastyGo.Interfaces.Services;
using TastyGo.Models;
using TastyGo.Utils;

namespace TastyGo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConstants _constants;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IUserRepository userRepository, IConstants constants, IMapper mapper, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _constants = constants;


        }

        public async Task<ServiceResponse<object>> RegisterAsync(RegisterRequestDto registerRequestDto)

        // check if the user type is valid
        {
            if (registerRequestDto.UserType != UserType.User && registerRequestDto.UserType != UserType.Vendor && registerRequestDto.UserType != UserType.Driver)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid user type specified.", AppStatusCode.VALIDATION_ERROR, null);
            }
            // Validaate that passwords match
            if (registerRequestDto.Password != registerRequestDto.ConfirmPassword)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Passwords do not match.", AppStatusCode.VALIDATION_ERROR, null);
            }
            // Check if email already exists
            var alreadyExistingUser = await _userRepository.GetByEmailAsync(registerRequestDto.Email);
            if (alreadyExistingUser != null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Email already exists.", AppStatusCode.EMAIL_ALREADY_EXISTS, null);
            }

            // user type specific validations
            switch (registerRequestDto.UserType)
            {
                case UserType.Driver:
                    {
                        // Ensure both LicenseNumber and NIN are provided
                        if (string.IsNullOrWhiteSpace(registerRequestDto.LicenseNumber) || string.IsNullOrWhiteSpace(registerRequestDto.NIN))
                        {
                            return new ServiceResponse<object>(ResponseStatus.BadRequest, "License number and NIN are required for drivers.", AppStatusCode.VALIDATION_ERROR, null);
                        }

                        // Verify BVN
                        var (isValidBvn, bvnMessage) = await VerifyBVN(new VerifyBvnDto
                        {
                            Bvn = registerRequestDto.NIN,
                            Name = $"{registerRequestDto.FirstName} {registerRequestDto.LastName}",
                            DateOfBirth = registerRequestDto.DateOfBirth,
                            MobileNo = registerRequestDto.Phone
                        });

                        if (!isValidBvn)
                            return new ServiceResponse<object>(ResponseStatus.BadRequest, bvnMessage, AppStatusCode.VALIDATION_ERROR, null);

                        // Verify License
                        var (isValidLicense, licenseMessage) = await VerifyLicense(new VerifyLicenseDto
                        {
                            LicenseNumber = registerRequestDto.LicenseNumber,
                            Name = $"{registerRequestDto.FirstName} {registerRequestDto.LastName}",
                            DateOfBirth = registerRequestDto.DateOfBirth
                        });

                        if (!isValidLicense)
                            return new ServiceResponse<object>(ResponseStatus.BadRequest, licenseMessage, AppStatusCode.VALIDATION_ERROR, null);

                        break;
                    }

                case UserType.Vendor:
                    {
                        // Optional: Add vendor-specific checks here later
                        break;
                    }

                case UserType.User:
                    {
                        // Optional: Add user-specific checks here later
                        break;
                    }
                default:
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid user type specified.", AppStatusCode.VALIDATION_ERROR, null);
            }

            // Map DTO to User model and hash the password
            User user;

            if (alreadyExistingUser is null)
            {

                user = _mapper.Map<User>(registerRequestDto);

                user.Id = Guid.NewGuid();

                user.Password = HashPassword(registerRequestDto.Password);
            }
            else
            {
                user = alreadyExistingUser;

            }

            // set email verification token and expiry
            var emailVerificationToken = RandomCharacterGenerator.GenerateRandomString(_constants.EMAIL_VERIFICATION_TOKEN_LENGTH);
            user.EmailVerificationToken = emailVerificationToken;
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(_constants.EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES);


            if (alreadyExistingUser is null)
            {
                _userRepository.Add(user);

            }

            // Committing user changes
            var userSaveResult = await _userRepository.SaveChangesAsync();
            if (!userSaveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to register user. Please try again later.", AppStatusCode.SERVER_ERROR, null);
            }

            // send email verification
            await SendVerificationMail(user, emailVerificationToken);

            return new ServiceResponse<object>(ResponseStatus.Success, "Registration was successful", AppStatusCode.CREATED, new
            {
                user = _mapper.Map<UserDto>(user),
            });
        }

        public async Task<ServiceResponse<object>> LoginAsync(LoginRequestDto request)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.ACCOUNT_NOT_FOUND, null);
            }
            // Verify the password
            if (!VerifyPassword(request.Password, user.Password))
            {
                return new ServiceResponse<object>(ResponseStatus.Unauthorized, "Invalid password.", AppStatusCode.INVALID_PASSWORD_EMAIL, null);
            }
            // Check if email is verified
            if (!user.IsEmailVerified)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Email not verified. Please verify your email before logging in.", AppStatusCode.EMAIL_NOT_VERIFIED, null);
            }
            // Create JWT token
            var jwtToken = CreateJwtToken(user);
            // Map user to UserDto
            var userDto = _mapper.Map<UserDto>(user);
            return new ServiceResponse<object>(ResponseStatus.Success, "Login successful.", AppStatusCode.SUCCESS, new
            {
                Token = jwtToken,
                User = userDto
            });

        }

        public async Task<ServiceResponse<object>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.ACCOUNT_NOT_FOUND, null);
            }
            // Generate a password reset token
            var resetToken = RandomCharacterGenerator.GenerateRandomString(_constants.PASSWORD_RESET_TOKEN_LENGTH);
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(_constants.PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES);
            _userRepository.Update(user);
            // Save changes to the database
            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to initiate password reset. Please try again later.", AppStatusCode.SERVER_ERROR, null);
            }
            // Send password reset email (implementation not shown here)
            await SendVerificationMail(user, resetToken);
            return new ServiceResponse<object>(ResponseStatus.Success, "Password reset token sent successfully.", AppStatusCode.PASSWORD_RESET_SENT, null);

        }

        public async Task<ServiceResponse<object>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
            // Generic failure to avoid leaking info
            if (user is null ||
                user.PasswordResetToken != resetPasswordDto.Token ||
                user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    "Invalid or expired token.",
                    AppStatusCode.TOKEN_INVALID,
                    null
                );
            }
            // Validate that new passwords match
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "New passwords do not match.", AppStatusCode.VALIDATION_ERROR, null);
            }
            // Hash the new password
            user.Password = HashPassword(resetPasswordDto.NewPassword);
            // Clear the reset token and expiry
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            // Update the user in the repository
            _userRepository.Update(user);
            // Save changes to the database
            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to reset password. Please try again later.", AppStatusCode.SERVER_ERROR, null);
            }
            return new ServiceResponse<object>(ResponseStatus.Success, "Password reset successful.", AppStatusCode.SUCCESS, null);

        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }


        public Task SendPasswordResetTokenAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<object>> VerifyEmail(EmailVerifyDto emailVerifyDto)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(emailVerifyDto.Email);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.ACCOUNT_NOT_FOUND, null);
            }
            // Check if the token matches
            if (user.EmailVerificationToken != emailVerifyDto.Token)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid verification token.", AppStatusCode.TOKEN_INVALID, null);
            }
            // Check if the token has expired
            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Verification token has expired.", AppStatusCode.TOKEN_EXPIRED, null);
            }
            // Update user verification status
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            // Save changes to the database
            _userRepository.Update(user);
            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to verify email. Please try again later.", AppStatusCode.SERVER_ERROR, null);
            }

            return new ServiceResponse<object>(ResponseStatus.Success, "Email verification successful.", AppStatusCode.SUCCESS, null);


        }

        private string CreateJwtToken(User user)
        {

            // Declaring claims we would like to write to the JWT
            List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // new Claim(ClaimTypes.Role, user.Role.ToString())

        };

            // Creating a new SymmetricKey from Token we have saved in appSettings.development.json file
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Token").Value));

            // Declaring signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Creating new JWT object
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            // Write JWT to a string
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private string HashPassword(string password)
        {

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private bool VerifyPassword(string password, string passwordHash)
        {

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private async Task<(bool, string)> VerifyBVN(VerifyBvnDto verifyBvnDto)
        {
            var message = "BVN verification is successful.";
            await Task.Delay(0); // Simulate async operation

            return (true, message); // Placeholder for actual verification logic
        }
        private async Task<(bool, string)> VerifyLicense(VerifyLicenseDto verifyLicenseDto)
        {
            var message = "License verification is successful.";
            await Task.Delay(0); // Simulate async operation
            return (true, message); // Placeholder for actual verification logic
        }

        private async Task<bool> SendVerificationMail(User user, string token)
        {
            // Placeholder for email sending logic
            _logger.LogInformation($"Sending verification email to {user.Email} with token {token}");
            await Task.Delay(0);
            // Simulate email sent successfully
            return true;

        }


    }
}
