using TastyGo.Models;

namespace TastyGo.Interfaces.Services
{
    public interface IMenuService
    {
        Task<Menu?> GetMenuByIdAsync(Guid id);
        Task<IEnumerable<Menu>> GetMenusByRestaurantIdAsync(Guid restaurantId);
        Task<bool> CreateMenuAsync(Menu menu);
        Task<bool> UpdateMenuAsync(Menu menu);
        Task<bool> DeleteMenuAsync(Guid id);
    }
}
