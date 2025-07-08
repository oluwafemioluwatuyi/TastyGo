using AutoMapper;
using TastyGo.DTOs;
using TastyGo.Helpers;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.Services;

namespace TastyGo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;

        }

        public Task<ServiceResponse<object>> RegisterAsync(RegisterRequestDto request)
        {
            throw new NotImplementedException();
        }
        public Task<bool> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<object>> LoginAsync(LoginRequestDto request)
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
