using Microsoft.EntityFrameworkCore;
using TastyGo.Data;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Models;

namespace TastyGo.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly TastyGoDbContext _dbContext;

        public MenuRepository(TastyGoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Menu?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Menus.Include(m => m.Restaurant)
                                         .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Menu>> GetAllByRestaurantIdAsync(Guid restaurantId)
        {
            return await _dbContext.Menus
                                   .Where(m => m.RestaurantId == restaurantId)
                                   .ToListAsync();
        }

        public async Task AddAsync(Menu menu)
        {
            await _dbContext.Menus.AddAsync(menu);
        }
        public void MarkAsModified(Menu menu)
        {
            _dbContext.Entry(menu).State = EntityState.Modified;
        }

        public void Delete(Menu menu)
        {
            _dbContext.Menus.Remove(menu);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
