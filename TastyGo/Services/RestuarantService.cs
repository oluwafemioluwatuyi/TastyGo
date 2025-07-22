using AutoMapper;
using TastyGo.DTOs;
using TastyGo.DTOs.SearchParams;
using TastyGo.Helpers;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.Services;
using TastyGo.Models;

namespace TastyGo.Services

{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserContextService _userContextService;
        private readonly IVendorRepository _vendorRepository;
        private readonly IMapper _mapper;
        public RestaurantService(IUserRepository userRepository, IRestaurantRepository restaurantRepository, IUserContextService userContextService, IMapper mapper, IVendorRepository vendorRepository)
        {
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
            _vendorRepository = vendorRepository;
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
            var vendor = await _vendorRepository.GetByUserIdAsync(userId);
            if (vendor is null)
            {
                return new ServiceResponse<object>(ResponseStatus.Forbidden, "User is not a restaurant owner.", AppStatusCode.Forbidden, null);
            }
            // Create the restaurant entity
            var restaurant = _mapper.Map<Restaurant>(createRestaurantDto);
            // Set the vendor ID from the user context
            restaurant.VendorId = vendor.Id;
            restaurant.ModifiedAt = DateTime.UtcNow;
            restaurant.CreatedAt = DateTime.UtcNow;
            restaurant.CreatedById = userId;
            restaurant.ModifiedById = userId;

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
            // Check if the user is a restaurant owner
            var vendor = await _vendorRepository.GetByUserIdAsync(userId);
            if (vendor == null)
            {
                return new ServiceResponse<bool>(ResponseStatus.Forbidden, "User is not a vendor.", AppStatusCode.Forbidden, false);
            }

            // Find the restaurant by ID
            var restaurant = await _restaurantRepository.FindByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return new ServiceResponse<bool>(ResponseStatus.NotFound, "Restaurant not found.", AppStatusCode.AccountNotFound, false);
            }
            // Check if the restaurant belongs to the vendor
            if (restaurant.VendorId != vendor.Id)
            {
                return new ServiceResponse<bool>(ResponseStatus.Forbidden, "Not authorized to delete this restaurant.", AppStatusCode.Forbidden, false);
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

            // public for both vendor, user, and admin

            var userId = _userContextService.UserId;
            if (userId == null)
            {
                return new ServiceResponse<object?>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
            }

            var restaurant = await _restaurantRepository.FindByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return new ServiceResponse<object?>(
                    ResponseStatus.NotFound,
                    "Restaurant not found.",
                    AppStatusCode.AccountNotFound,
                    null
                );
            }

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return new ServiceResponse<object?>(
                ResponseStatus.Success,
                "Restaurant retrieved successfully.",
                AppStatusCode.Success,
                restaurantDto
            );

        }

        public async Task<ServiceResponse<object>> UpdateRestaurantAsync(UpdateRestaurantRequestDto updateRestaurantRequestDto, Guid restaurantId)
        {

            var userId = _userContextService.UserId;
            if (userId == null)
            {
                return new ServiceResponse<object>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
            }

            var vendor = await _vendorRepository.GetByUserIdAsync(userId);
            if (vendor == null)
            {
                return new ServiceResponse<object>(ResponseStatus.Forbidden, "User is not a vendor.", AppStatusCode.Forbidden, null);
            }

            var restaurant = await _restaurantRepository.FindByIdAsync(restaurantId);
            if (restaurant == null)
            {
                return new ServiceResponse<object>(ResponseStatus.NotFound, "Restaurant not found.", AppStatusCode.AccountNotFound, null);
            }

            // Check if the vendor owns the restaurant
            if (restaurant.VendorId != vendor.Id)
            {
                return new ServiceResponse<object>(ResponseStatus.Forbidden, "Not authorized to update this restaurant.", AppStatusCode.Forbidden, null);
            }

            // Map updated values onto existing restaurant entity
            _mapper.Map(updateRestaurantRequestDto, restaurant);
            _restaurantRepository.MarkAsModified(restaurant);

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            var success = await _restaurantRepository.SaveChangesAsync();
            if (!success)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, "Failed to update restaurant.", AppStatusCode.InternalServerError, null);
            }


            return new ServiceResponse<object>(ResponseStatus.Success, "Restaurant updated successfully.", AppStatusCode.Success, restaurantDto);

        }
    }
}