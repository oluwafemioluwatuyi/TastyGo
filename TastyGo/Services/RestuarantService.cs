using AutoMapper;
using TastyGo.DTOs;
using TastyGo.DTOs.SearchParams;
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
        private readonly IMapper _mapper;
        public RestaurantService(IUserRepository userRepository, IRestaurantRepository restaurantRepository, IUserContextService userContextService, IMapper mapper)
        {
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
            _userContextService = userContextService;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<object>> CreateRestaurantAsync(CreateRestaurantDto createRestaurantDto)
        {
            // Validate the user context
            var userId = _userContextService.UserId;
            if (userId == null)
            {
                return new ServiceResponse<object>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
            }
            // Check if the user is a restaurant owner
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.userType is not UserType.Vendor)
            {
                return new ServiceResponse<object>(ResponseStatus.Forbidden, "User is not a restaurant owner.", AppStatusCode.Forbidden, null);
            }
            // Create the restaurant entity
            var restaurant = _mapper.Map<Restaurant>(createRestaurantDto);
            restaurant.UserId = user.Id;

            _restaurantRepository.Add(restaurant);

            // Save changes to the database
            await _restaurantRepository.SaveChangesAsync();
            // Map the created restaurant to a DTO
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return new ServiceResponse<object>(ResponseStatus.Success, "Restaurant created successfully.", AppStatusCode.Created, new
            {
                Restaurant = restaurantDto

            });
        }

        public async Task<ServiceResponse<bool>> DeleteRestaurantAsync(Guid restaurantId)
        {
            // Validate the user context
            var userId = _userContextService.UserId;
            if (userId == null)
            {
                return new ServiceResponse<bool>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, false);
            }
            // Check if the restaurant exists

            var restaurant = await _restaurantRepository.FindByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return new ServiceResponse<bool>(ResponseStatus.NotFound, "Restaurant not found.", AppStatusCode.AccountNotFound, false);
            }
            // Check if the user is the owner of the restaurant
            if (restaurant.UserId != userId)
            {
                return new ServiceResponse<bool>(ResponseStatus.Forbidden, "User is not authorized to delete this restaurant.", AppStatusCode.Forbidden, false);
            }
            // Delete the restaurant
            _restaurantRepository.Delete(restaurant);

            var success = await _restaurantRepository.SaveChangesAsync();
            if (!success)
            {
                return new ServiceResponse<bool>(ResponseStatus.Error, "Failed to delete restaurant.", AppStatusCode.InternalServerError, false);
            }
            return new ServiceResponse<bool>(ResponseStatus.Success, "Restaurant deleted successfully.", AppStatusCode.Success, true);

        }

        public Task<ServiceResponse<IEnumerable<object>>> GetAllRestaurantsAsync(RestaurantSearchParamsDto searchParamsDto)
        {
            throw new NotImplementedException();
        }
        public Task<ServiceResponse<IEnumerable<object>>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusInKm)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<object?>> GetRestaurantByIdAsync(Guid restaurantId)
        {

            var userId = _userContextService.UserId;
            if (userId == null)
            {
                return new ServiceResponse<object?>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
            }

            var restaurant = await _restaurantRepository.FindByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return new ServiceResponse<object?>(ResponseStatus.NotFound, "Restaurant not found.", AppStatusCode.AccountNotFound, null);
            }

            if (restaurant.UserId != userId)
            {
                return new ServiceResponse<object?>(ResponseStatus.Forbidden, "User is not authorized to view this restaurant.", AppStatusCode.Forbidden, null);
            }

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return new ServiceResponse<object?>(ResponseStatus.Success, "Restaurant retrieved successfully.", AppStatusCode.Success, restaurantDto);
        }

        public async Task<ServiceResponse<object>> UpdateRestaurantAsync(UpdateRestaurantRequestDto updateRestaurantRequestDto, Guid restaurantId)
        {

            var userId = _userContextService.UserId;
            if (userId == null)
            {
                return new ServiceResponse<object>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
            }

            // Check if the user is a restaurant owner
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.userType is not UserType.Vendor)
            {
                return new ServiceResponse<object>(ResponseStatus.Forbidden, "User is not a restaurant owner.", AppStatusCode.Forbidden, null);
            }
            // Find the restaurant by ID
            var restaurant = await _restaurantRepository.FindByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "Restaurant not found.", AppStatusCode.AccountNotFound, null);
            }
            // Check if the user is the owner of the restaurant
            if (restaurant.UserId != userId)
            {
                return new ServiceResponse<object>(ResponseStatus.Forbidden, "User is not authorized to update this restaurant.", AppStatusCode.Forbidden, null);
            }

            _mapper.Map(updateRestaurantRequestDto, restaurant);

            _restaurantRepository.MarkAsModified(restaurant);

            var success = await _restaurantRepository.SaveChangesAsync();
            if (!success)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to update restaurant.", AppStatusCode.InternalServerError, null);
            }

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return new ServiceResponse<object>(ResponseStatus.Success, "Restaurant updated successfully.", AppStatusCode.Success, restaurantDto);

        }
    }
}