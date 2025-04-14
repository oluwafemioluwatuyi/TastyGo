namespace TastyGo.Interfaces.Services
{
    public interface IAuthService
    {
        // Registration
        //Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

        // Login
        //Task<AuthResponseDto> LoginAsync(LoginRequestDto request);

        // Logout (optional in stateless JWT scenarios)
        Task LogoutAsync();

        // Refresh token (if you're using JWT with refresh tokens)
        //Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

        // Email verification
        Task<bool> VerifyEmailAsync(string token);

        // Send email verification
        Task SendEmailVerificationAsync(string email);

        // Forgot Password
        Task SendPasswordResetTokenAsync(string email);

        // Reset Password
        //Task<bool> ResetPasswordAsync(ResetPasswordDto request);

        // Check if email exists (before register or reset password)
        Task<bool> EmailExistsAsync(string email);

        // Optional: Get current logged-in user (from token context)
        //Task<UserDto> GetLoggedInUserAsync();
    }

}

