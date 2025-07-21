using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastyGo.DTOs.SearchParams;
using TastyGo.Models;

namespace TastyGo.Interfaces.IRepositories
{
    public interface IRestaurantRepository
    {
        void Add(Restaurant restaurant);

        void MarkAsModified(Restaurant restaurant);
        void Delete(Restaurant restaurant);
        Task<Restaurant?> FindByIdAsync(Guid restaurantId);
        Task<IEnumerable<Restaurant>> GetAllAsync(RestaurantSearchParamsDto searchParamsDto);

        Task<IEnumerable<Restaurant>> GetNearbyAsync(double latitude, double longitude, double radiusInKm);
        Task<bool> RestaurantExistsAsync(string name);
        Task<bool> SaveChangesAsync();
    }
}
