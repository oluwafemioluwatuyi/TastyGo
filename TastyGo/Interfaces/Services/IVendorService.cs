using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TastyGo.DTOs;
using TastyGo.DTOs.Vendor;
using TastyGo.Helpers;
using TastyGo.Models;

namespace TastyGo.Interfaces.Services;

public interface IVendorService
{
    Task<ServiceResponse<object>> CreateVendorAsync(CreateVendorDto createVendorDto);
    Task<ServiceResponse<object>> UpdateVendorAsync(UpdateVendorRequestDto updateVendorRequestDto, Guid vendorId);
    Task<ServiceResponse<bool>> DeleteVendorAsync(Guid vendorId);
    Task<ServiceResponse<object?>> GetVendorByIdAsync(Guid vendorId);
    Task<ServiceResponse<IEnumerable<object>>> GetAllVendorsAsync();
    Task<ServiceResponse<object?>> GetVendorByUserIdAsync(Guid userId);
    Task<ServiceResponse<bool>> VendorExistsAsync(Guid userId);
    Task<ServiceResponse<object>> GetVendorProfileAsync(Guid userId);
}
