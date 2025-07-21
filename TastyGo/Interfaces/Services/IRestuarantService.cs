using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastyGo.DTOs;
using TastyGo.Helpers;
using TastyGo.Models;

namespace TastyGo.Interfaces.IServices
{
    public interface IRestaurantService
    {
        Task<ServiceResponse<object>> CreateRestaurantAsync(CreateRestaurantDto createRestaurantDto);

        Task<ServiceResponse<object>> UpdateRestaurantAsync(UpdateRestaurantRequestDto updateRestaurantRequestDto, Guid restaurantId);

        Task<ServiceResponse<bool>> DeleteRestaurantAsync(Guid restaurantId);

        Task<ServiceResponse<object?>> GetRestaurantByIdAsync(Guid restaurantId);

        Task<ServiceResponse<IEnumerable<object>>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusInKm);
    }
}
