using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastyGo.Helpers;
using TastyGo.Models;

namespace TastyGo.Interfaces.IServices
{
    public interface IRestaurantService
    {
        Task<ServiceResponse<Restaurant>> CreateRestaurantAsync(Restaurant restaurant);

        Task<ServiceResponse<Restaurant>> UpdateRestaurantAsync(Restaurant restaurant);

        Task<ServiceResponse<bool>> DeleteRestaurantAsync(Guid restaurantId);

        Task<ServiceResponse<Restaurant?>> GetRestaurantByIdAsync(Guid restaurantId);

        Task<ServiceResponse<IEnumerable<Restaurant>>> GetAllRestaurantsAsync();

        Task<ServiceResponse<IEnumerable<Restaurant>>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusInKm);

        Task<ServiceResponse<bool>> RestaurantExistsAsync(string name);
    }
}
