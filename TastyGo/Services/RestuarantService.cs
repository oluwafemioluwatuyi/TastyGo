using TastyGo.Helpers;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.IServices;
using TastyGo.Interfaces.Services;
using TastyGo.Models;

namespace TastyGo.Services

{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserContextService _userContextService;
        public RestaurantService(IUserRepository userRepository, IRestaurantRepository restaurantRepository, IUserContextService userContextService)
        {
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
            _userContextService = userContextService;
            // Initialize any dependencies here, e.g., repository, logger, etc.
        }
        public Task<ServiceResponse<Restaurant>> CreateRestaurantAsync(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteRestaurantAsync(Guid restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<IEnumerable<Restaurant>>> GetAllRestaurantsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<IEnumerable<Restaurant>>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusInKm)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Restaurant?>> GetRestaurantByIdAsync(Guid restaurantId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> RestaurantExistsAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Restaurant>> UpdateRestaurantAsync(Restaurant restaurant)
        {
            throw new NotImplementedException();
        }
    }
}