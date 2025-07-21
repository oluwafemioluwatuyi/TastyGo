
using Microsoft.EntityFrameworkCore;
using TastyGo.Data;
using TastyGo.DTOs.SearchParams;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Models;

namespace TastyGo.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly TastyGoDbContext _dbContext;
        public RestaurantRepository(TastyGoDbContext _dbContext)
        {
            this._dbContext = _dbContext;

            // Initialize any required services or context here
        }
        public void Add(Restaurant restaurant)
        {
            _dbContext.Restaurants.Add(restaurant);
        }

        public void Delete(Restaurant restaurant)
        {
            _dbContext.Restaurants.Remove(restaurant);
        }

        public async Task<Restaurant?> FindByIdAsync(Guid restaurantId)
        {
            return await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId);
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync(RestaurantSearchParamsDto searchParamsDto)
        {
            return await _dbContext.Restaurants
                .Include(r => r.Menus)
                .Include(r => r.Orders)
                .ToListAsync();
        }

        public Task<IEnumerable<Restaurant>> GetNearbyAsync(double latitude, double longitude, double radiusInKm)
        {
            throw new NotImplementedException();
        }

        public void MarkAsModified(Restaurant restaurant)
        {
            _dbContext.Entry(restaurant).State = EntityState.Modified;
        }

        public async Task<bool> RestaurantExistsAsync(string name)
        {
            return await _dbContext.Restaurants.AnyAsync(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}