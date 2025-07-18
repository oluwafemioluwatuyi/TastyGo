using TastyGo.DTOs;
using TastyGo.Helpers;

namespace TastyGo.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<object>> RegisterAsync(RegisterRequestDto request);
        Task<ServiceResponse<object>> LoginAsync(LoginRequestDto request);
        Task LogoutAsync();
        Task<ServiceResponse<object>> VerifyEmail(VerifyEmailRequestDto verifyEmailRequestDto);
        Task<ServiceResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordDto);
        Task<ServiceResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto);
        Task<ServiceResponse<object>> CreatePin(CreatePinRequestDto createPinDto);
        Task<ServiceResponse<object>> ResetPin(ResetPinRequestDto resetPinDto);

        // Forgot Password
        Task SendPasswordResetTokenAsync(string email);

        // Reset Password
        //Task<bool> ResetPasswordAsync(ResetPasswordDto request);

        // Check if email exists (before register or reset password)


        // Optional: Get current logged-in user (from token context)
        //Task<UserDto> GetLoggedInUserAsync();
    }

}

