using TastyGo.Interfaces.Services;

namespace TastyGo.Services
{
    public class AuthService : IAuthService
    {
        public Task<bool> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendEmailVerificationAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetTokenAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmailAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}
