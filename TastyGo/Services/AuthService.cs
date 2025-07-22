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
using TastyGo.Interfaces.Repositories;
using TastyGo.Interfaces.Services;
using TastyGo.Models;
using TastyGo.Utils;

namespace TastyGo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConstants _constants;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IUserContextService _userContextService;
        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IConstants constants, IMapper mapper, IConfiguration configuration, ILogger<AuthService> logger, IUserContextService userContextService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _constants = constants;
            _userContextService = userContextService;


        }

        public async Task<ServiceResponse<object>> RegisterAsync(RegisterRequestDto registerRequestDto)

        // check if the user type is valid
        {
            if (registerRequestDto.UserType != UserType.User && registerRequestDto.UserType != UserType.Vendor && registerRequestDto.UserType != UserType.Driver)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid user type specified.", AppStatusCode.ValidationError, null);
            }
            // Validaate that passwords match
            if (registerRequestDto.Password != registerRequestDto.ConfirmPassword)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Passwords do not match.", AppStatusCode.ValidationError, null);
            }
            // Check if email already exists
            var alreadyExistingUser = await _userRepository.GetByEmailAsync(registerRequestDto.Email);
            if (alreadyExistingUser != null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Email already exists.", AppStatusCode.EmailAlreadyExists, null);
            }

            // user type specific validations
            switch (registerRequestDto.UserType)
            {
                case UserType.Driver:
                    {
                        // Ensure both LicenseNumber and NIN are provided
                        if (string.IsNullOrWhiteSpace(registerRequestDto.LicenseNumber) || string.IsNullOrWhiteSpace(registerRequestDto.NIN))
                        {
                            return new ServiceResponse<object>(ResponseStatus.BadRequest, "License number and NIN are required for drivers.", AppStatusCode.ValidationError, null);
                        }

                        // Verify BVN
                        var (isValidNin, ninMessage) = await VerifyNIN(new VerifyNinDto
                        {
                            Nin = registerRequestDto.NIN,
                            Name = $"{registerRequestDto.FirstName} {registerRequestDto.LastName}",
                            DateOfBirth = registerRequestDto.DateOfBirth,
                            MobileNo = registerRequestDto.Phone
                        });

                        if (!isValidNin)
                            return new ServiceResponse<object>(ResponseStatus.BadRequest, ninMessage, AppStatusCode.ValidationError, null);

                        // Verify License
                        var (isValidLicense, licenseMessage) = await VerifyLicense(new VerifyLicenseDto
                        {
                            LicenseNumber = registerRequestDto.LicenseNumber,
                            Name = $"{registerRequestDto.FirstName} {registerRequestDto.LastName}",
                            DateOfBirth = registerRequestDto.DateOfBirth
                        });

                        if (!isValidLicense)
                            return new ServiceResponse<object>(ResponseStatus.BadRequest, licenseMessage, AppStatusCode.ValidationError, null);

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
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid user type specified.", AppStatusCode.ValidationError, null);
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
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to register user. Please try again later.", AppStatusCode.InternalServerError, null);
            }

            // send email verification
            await SendVerificationMail(user, emailVerificationToken);

            return new ServiceResponse<object>(ResponseStatus.Success, "Registration was successful", AppStatusCode.Success, new
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
                return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.AccountNotFound, null);
            }
            // Verify the password
            if (!VerifyPassword(request.Password, user.Password))
            {
                return new ServiceResponse<object>(ResponseStatus.Unauthorized, "Invalid password.", AppStatusCode.InvalidPasswordOrEmail, null);
            }
            // Check if email is verified
            if (!user.IsEmailVerified)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Email not verified. Please verify your email before logging in.", AppStatusCode.EmailNotVerified, null);
            }
            // Create JWT token
            // 4. Create Access Token (JWT)
            var jwtToken = CreateJwtToken(user);
            // Decode the JWT to get the JTI (JWT ID)
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtDecoded = jwtHandler.ReadJwtToken(jwtToken);
            var jti = jwtDecoded.Id;

            // 5. Create and Save Refresh Token
            var refreshToken = new RefreshToken
            {
                Token = RandomCharacterGenerator.GenerateRandomString(_constants.REFRESH_TOKEN_LENGTH),
                JwtId = jti,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_constants.REFRESH_TOKEN_EXPIRATION_DAYS),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = user.Id,         // ✅ Required by AuditableEntity
                ModifiedById = user.Id,
                Used = false,
                Invalidated = false
            };

            _refreshTokenRepository.Add(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
            // Map user to UserDto
            var userDto = _mapper.Map<UserDto>(user);
            return new ServiceResponse<object>(ResponseStatus.Success, "Login successful.", AppStatusCode.Success, new
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token,
                User = userDto
            });

        }

        public async Task<ServiceResponse<object>> RefreshTokenAsync(string? existingToken = null)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(existingToken);

            if (storedToken == null || storedToken.Used || storedToken.Invalidated || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                if (storedToken != null)
                {
                    // Delete the invalid/expired/revoked token
                    _refreshTokenRepository.Delete(storedToken);
                    await _refreshTokenRepository.SaveChangesAsync();
                }
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid refresh token", AppStatusCode.InvalidToken, null);
            }

            // Get user from token
            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "User not found", AppStatusCode.ValidationError, null);
            }

            // Mark token as used
            storedToken.Used = true;
            _refreshTokenRepository.MarkAsModified(storedToken);

            var newAccessToken = CreateJwtToken(user);
            // Decode the JWT to get the JTI (JWT ID)
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtDecoded = jwtHandler.ReadJwtToken(newAccessToken);
            var jti = jwtDecoded.Id;

            // Generate new refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = RandomCharacterGenerator.GenerateRandomString(_constants.REFRESH_TOKEN_LENGTH),
                JwtId = jti,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_constants.REFRESH_TOKEN_EXPIRATION_DAYS),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = user.Id,         // ✅ Required by AuditableEntity
                ModifiedById = user.Id,
                Used = false,
                Invalidated = false
            };

            _refreshTokenRepository.Add(newRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            var result = new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

            return new ServiceResponse<object>(ResponseStatus.Success, "Token refreshed successfully", AppStatusCode.Success, result);
        }

        public async Task<ServiceResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(forgotPasswordRequestDto.Email);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.AccountNotFound, null);
            }
            // Generate a password reset token
            var resetToken = RandomCharacterGenerator.GenerateRandomString(_constants.PASSWORD_RESET_TOKEN_LENGTH);
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(_constants.PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES);
            _userRepository.MarkAsModified(user);
            // Save changes to the database
            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to initiate password reset. Please try again later.", AppStatusCode.InternalServerError, null);
            }
            // Send password reset email (implementation not shown here)
            await SendVerificationMail(user, resetToken);
            return new ServiceResponse<object>(ResponseStatus.Success, "Password reset token sent successfully.", AppStatusCode.PasswordResetSent, null);

        }

        public async Task<ServiceResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(resetPasswordRequestDto.Email);
            // Generic failure to avoid leaking info
            if (user is null ||
                user.PasswordResetToken != resetPasswordRequestDto.Token ||
                user.PasswordResetTokenExpiry < DateTime.UtcNow)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    "Invalid or expired token.",
                    AppStatusCode.InvalidToken,
                    null
                );
            }
            // Validate that new passwords match
            if (resetPasswordRequestDto.NewPassword != resetPasswordRequestDto.ConfirmPassword)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "New passwords do not match.", AppStatusCode.ValidationError, null);
            }
            // Hash the new password
            user.Password = HashPassword(resetPasswordRequestDto.NewPassword);
            // Clear the reset token and expiry
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            // Update the user in the repository
            _userRepository.MarkAsModified(user);
            // Save changes to the database
            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to reset password. Please try again later.", AppStatusCode.InternalServerError, null);
            }
            return new ServiceResponse<object>(ResponseStatus.Success, "Password reset successful.", AppStatusCode.Success, null);

        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }


        public Task SendPasswordResetTokenAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<object>> VerifyEmail(VerifyEmailRequestDto verifyEmailRequestDto)
        {
            // Check if the user exists
            var user = await _userRepository.GetByEmailAsync(verifyEmailRequestDto.Email);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.AccountNotFound, null);
            }
            // Check if the token matches
            if (user.EmailVerificationToken != verifyEmailRequestDto.Token)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid verification token.", AppStatusCode.InvalidToken, null);
            }
            // Check if the token has expired
            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Verification token has expired.", AppStatusCode.TokenExpired, null);
            }
            // Update user verification status
            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;

            // Save changes to the database
            _userRepository.MarkAsModified(user);
            var saveResult = await _userRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to verify email. Please try again later.", AppStatusCode.InternalServerError, null);
            }

            return new ServiceResponse<object>(ResponseStatus.Success, "Email verification successful.", AppStatusCode.Success, null);


        }

        public async Task<ServiceResponse<object>> CreatePin(CreatePinRequestDto createPinDto)
        {
            var userId = _userContextService.UserId;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "User does not exist", AppStatusCode.ResourceNotFound, null);
            }
            // 2. Validate if PIN already exists (optional: skip if you want to allow overwrite)
            if (user.PinCreated)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    "PIN already created. Please reset it instead.",
                    AppStatusCode.PinAlreadyCreated,
                    null
                );
            }

            // 3. Validate PIN match
            if (createPinDto.Pin != createPinDto.ConfirmPin)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    "PINs do not match.",
                    AppStatusCode.ValidationError,
                    null
                );
            }
            // Validate the format of the pin
            var pinOnlyContainsNumber = Utilities.IsStringNumericRegex(createPinDto.Pin);
            if (!pinOnlyContainsNumber)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid pin format", AppStatusCode.ValidationError, null);
            }


            // 4. Hash the PIN (you can reuse your `HashPassword()` method)
            user.Pin = HashPassword(createPinDto.Pin);
            user.PinCreated = true;
            user.ModifiedAt = DateTime.UtcNow;

            _userRepository.MarkAsModified(user);

            var result = await _userRepository.SaveChangesAsync();
            if (!result)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.Error,
                    "Something went wrong while creating PIN.",
                    AppStatusCode.InternalServerError,
                    null
                );
            }

            return new ServiceResponse<object>(
                ResponseStatus.Success,
                "PIN created successfully.",
                AppStatusCode.Created,
                null
            );

        }

        public async Task<ServiceResponse<object>> ResetPin(ResetPinRequestDto resetPinDto)
        {
            var userId = _userContextService.UserId;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "User does not exist", AppStatusCode.ResourceNotFound, null);
            }

            // Check if user has created a pin
            if (!user.PinCreated)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "User does not have a pin", AppStatusCode.NoPinCreated, null);
            }

            //Confirm if old pin is correct
            var isOldPinCorrect = VerifyPassword(resetPinDto.OldPin, user.Pin);

            if (!isOldPinCorrect)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid credentials", AppStatusCode.ValidationError, null);

            }

            // Check if pin match
            if (resetPinDto.NewPin != resetPinDto.NewPin)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Pin does not match", AppStatusCode.ValidationError, null);
            }

            //Validate that pin is in the right format
            var pinOnlyContainsNumbers = Utilities.IsStringNumericRegex(resetPinDto.NewPin);

            if (!pinOnlyContainsNumbers)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, "Invalid pin format", AppStatusCode.ValidationError, null);
            }

            // Change pin
            user.Pin = HashPassword(resetPinDto.NewPin);
            user.ModifiedAt = DateTime.UtcNow;

            _userRepository.MarkAsModified(user);
            var result = await _userRepository.SaveChangesAsync();
            if (!result)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Something went wrong", AppStatusCode.InternalServerError, null);
            }

            return new ServiceResponse<object>(ResponseStatus.Success, "Pin changed successfully", AppStatusCode.Success, null);
        }



        private string CreateJwtToken(User user)
        {

            // Declaring claims we would like to write to the JWT
            List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
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

        private async Task<(bool, string)> VerifyNIN(VerifyNinDto verifyNinDto)
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
