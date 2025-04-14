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
            throw new NotImplementedException();
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetSystemUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
