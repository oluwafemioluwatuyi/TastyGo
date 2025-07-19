using TastyGo.Models;

namespace TastyGo.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByJwtIdAsync(string jwtId);
        Task<RefreshToken> GetByUserIdAsync(Guid userId, string token);
        void Add(RefreshToken refreshToken);
        void MarkAsModified(RefreshToken refreshToken);
        void Delete(RefreshToken refreshToken);
        Task<bool> SaveChangesAsync();
        Task<bool> IsTokenValidAsync(string token);

    }
}