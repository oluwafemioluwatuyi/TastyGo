using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyGo.DTOs;
using TastyGo.DTOs.Vendor;
using TastyGo.Helpers;
using TastyGo.Interfaces;
using TastyGo.Interfaces.Services;


namespace TastyGo.Controllers;

[ApiController]
[Route("api/Vendor")]

public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;

    }

    [HttpPost("create-vendor")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateVendorDto createVendorDto)
    {
        var response = await _vendorService.CreateVendorAsync(createVendorDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteVendor(Guid id)
    {
        var response = await _vendorService.DeleteVendorAsync(id);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetVendor(Guid id)
    {
        var response = await _vendorService.GetVendorByIdAsync(id);
        return ControllerHelper.HandleApiResponse(response);
    }




}
