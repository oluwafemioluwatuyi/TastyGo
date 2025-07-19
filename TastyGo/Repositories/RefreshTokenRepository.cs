
using Microsoft.EntityFrameworkCore;
using TastyGo.Data;
using TastyGo.Interfaces.Repositories;
using TastyGo.Models;

namespace TastyGo.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TastyGoDbContext _dbContext;

        public RefreshTokenRepository(TastyGoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Add(refreshToken);

        }

        public void Delete(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Remove(refreshToken);
        }

        public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.JwtId == jwtId);

        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<RefreshToken> GetByUserIdAsync(Guid userId, string token)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);
        }

        public Task<bool> IsTokenValidAsync(string token)
        {
            return _dbContext.RefreshTokens.AnyAsync(x => x.Token == token && !x.Invalidated && !x.Used && x.ExpiryDate > DateTime.UtcNow);

        }

        public void MarkAsModified(RefreshToken refreshToken)
        {
            _dbContext.Entry(refreshToken).State = EntityState.Modified;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}