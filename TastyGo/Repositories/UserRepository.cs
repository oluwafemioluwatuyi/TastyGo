using Microsoft.EntityFrameworkCore;
using TastyGo.Data;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Models;

namespace TastyGo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private TastyGoDbContext DbContext;
        public UserRepository(TastyGoDbContext DbContext)
        {
            this.DbContext = DbContext;

        }
        public void Add(User user)
        {
            DbContext.Users.Add(user);
        }

        public void Delete(User user)
        {
            DbContext.Users.Remove(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await DbContext.Users.AnyAsync(u => u.Email == email);

        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await DbContext.Users.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await DbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await DbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        }

        public Task<User?> GetSystemUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync() > 0;
        }

        public void MarkAsModified(User user)
        {
            DbContext.Entry(user).State = EntityState.Modified;
        }
    }
}
