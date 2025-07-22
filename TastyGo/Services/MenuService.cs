using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.Services;
using TastyGo.Models;

namespace TastyGo.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<Menu?> GetMenuByIdAsync(Guid id)
        {
            return await _menuRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Menu>> GetMenusByRestaurantIdAsync(Guid restaurantId)
        {
            return await _menuRepository.GetAllByRestaurantIdAsync(restaurantId);
        }

        public async Task<bool> CreateMenuAsync(Menu menu)
        {
            await _menuRepository.AddAsync(menu);
            return await _menuRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateMenuAsync(Menu menu)
        {
            _menuRepository.MarkAsModified(menu);
            return await _menuRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteMenuAsync(Guid id)
        {
            var existingMenu = await _menuRepository.GetByIdAsync(id);
            if (existingMenu == null) return false;

            _menuRepository.Delete(existingMenu);
            return await _menuRepository.SaveChangesAsync();
        }
    }
}
