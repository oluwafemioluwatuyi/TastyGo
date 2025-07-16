using TastyGo.Models;

namespace TastyGo.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetSystemUserAsync();
        void Add(User user);
        void MarkAsModified(User user);
        void Delete(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> SaveChangesAsync();
    }
}
