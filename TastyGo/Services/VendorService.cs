using AutoMapper;
using TastyGo.DTOs;
using TastyGo.DTOs.Vendor;
using TastyGo.Helpers;
using TastyGo.Interfaces.IRepositories;
using TastyGo.Interfaces.Services;
using TastyGo.Models;

namespace TastyGo.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IUserContextService _userContextService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public VendorService(IVendorRepository vendorRepository, IUserRepository userRepository, IUserContextService userContextService, IMapper mapper)
    {
        _vendorRepository = vendorRepository;
        _userRepository = userRepository;

        _userContextService = userContextService;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<object>> CreateVendorAsync(CreateVendorDto createVendorDto)
    {
        // validate userContext
        var currentUserId = _userContextService.UserId;
        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return new ServiceResponse<object>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
        }
        // Check if the user is a vendor
        var existingUser = await _userRepository.GetByIdAsync(currentUserId);
        if (existingUser == null || existingUser.userType != UserType.Vendor)
        {
            return new ServiceResponse<object>(ResponseStatus.NotFound, "User not found.", AppStatusCode.AccountNotFound, null);
        }
        // Create the vendor entity
        var vendor = _mapper.Map<Vendor>(createVendorDto);
        vendor.UserId = currentUserId;
        vendor.CreatedById = currentUserId;
        vendor.ModifiedById = currentUserId;
        vendor.CreatedAt = DateTime.UtcNow;
        vendor.ModifiedAt = DateTime.UtcNow;

        _vendorRepository.Add(vendor);

        await _vendorRepository.SaveChangesAsync();
        // Map the created vendor to a DTO
        var vendorDto = _mapper.Map<VendorDto>(vendor);
        return new ServiceResponse<object>(ResponseStatus.Success, "Vendor created successfully.", AppStatusCode.Created, new
        {
            Vendor = vendorDto
        });

    }

    public async Task<ServiceResponse<bool>> DeleteVendorAsync(Guid vendorId)
    {
        // Validate the user context
        var userId = _userContextService.UserId;
        if (userId == null)
        {
            return new ServiceResponse<bool>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, false);
        }
        // Check if the vendor exists
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        if (vendor == null || vendor.UserId != userId)
        {
            return new ServiceResponse<bool>(ResponseStatus.NotFound, "Vendor not found or does not belong to the user.", AppStatusCode.AccountNotFound, false);
        }

        _vendorRepository.Delete(vendor);

        var success = await _vendorRepository.SaveChangesAsync();
        if (!success)
        {
            return new ServiceResponse<bool>(ResponseStatus.Error, "Failed to delete vendor.", AppStatusCode.InternalServerError, false);
        }
        return new ServiceResponse<bool>(ResponseStatus.Success, "Vendor deleted successfully.", AppStatusCode.Success, true);

    }

    public Task<ServiceResponse<IEnumerable<object>>> GetAllVendorsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<object?>> GetVendorByIdAsync(Guid vendorId)
    {
        // Validate the user context
        var userId = _userContextService.UserId;
        if (userId == null)
        {
            return new ServiceResponse<object?>(ResponseStatus.Unauthorized, "User not authenticated.", AppStatusCode.Unauthorized, null);
        }
        // Get the vendor by ID
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        if (vendor == null || vendor.UserId != userId)
        {
            return new ServiceResponse<object?>(ResponseStatus.NotFound, "Vendor not found or does not belong to the user.", AppStatusCode.AccountNotFound, null);
        }
        // Map the vendor to a DTO
        var vendorDto = _mapper.Map<VendorDto>(vendor);
        return new ServiceResponse<object?>(ResponseStatus.Success, "Vendor retrieved successfully.", AppStatusCode.Success, vendorDto);
    }

    public Task<ServiceResponse<object?>> GetVendorByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<object>> GetVendorProfileAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<object>> UpdateVendorAsync(UpdateVendorRequestDto updateVendorRequestDto, Guid vendorId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<bool>> VendorExistsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
