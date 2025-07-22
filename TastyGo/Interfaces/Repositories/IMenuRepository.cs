using TastyGo.Models;

namespace TastyGo.Interfaces.IRepositories
{
    public interface IMenuRepository
    {
        Task<Menu?> GetByIdAsync(Guid id);
        Task<IEnumerable<Menu>> GetAllByRestaurantIdAsync(Guid restaurantId);
        Task AddAsync(Menu menu);
        void MarkAsModified(Menu menu);
        void Delete(Menu menu);
        Task<bool> SaveChangesAsync();
    }
}
