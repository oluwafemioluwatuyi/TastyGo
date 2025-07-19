
using TastyGo.Data;
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
            throw new NotImplementedException();
        }

        public void Delete(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public Task<Restaurant?> FindByIdAsync(Guid restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Restaurant>> GetNearbyAsync(double latitude, double longitude, double radiusInKm)
        {
            throw new NotImplementedException();
        }

        public void MarkAsModified(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RestaurantExistsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}