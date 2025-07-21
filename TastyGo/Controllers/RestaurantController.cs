using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyGo.DTOs;
using TastyGo.Helpers;
using TastyGo.Interfaces;
using TastyGo.Interfaces.IServices;


namespace TastyGo.Controllers;

[ApiController]
[Route("api/restaurants")]

public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService restaurantService;
    public RestaurantController(IRestaurantService restaurantService)
    {
        this.restaurantService = restaurantService;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto createRestaurantDto)
    {
        var response = await restaurantService.CreateRestaurantAsync(createRestaurantDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpDelete("{restaurantId}")]
    [Authorize]
    public async Task<IActionResult> DeleteRestaurant(Guid restaurantId)
    {
        var response = await restaurantService.DeleteRestaurantAsync(restaurantId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("{restaurantId}")]
    public async Task<IActionResult> GetRestaurantById(Guid restaurantId)
    {
        var response = await restaurantService.GetRestaurantByIdAsync(restaurantId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPut("{restaurantId}")]
    [Authorize]
    public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantRequestDto updateRestaurantRequestDto, Guid restaurantId)
    {
        var response = await restaurantService.UpdateRestaurantAsync(updateRestaurantRequestDto, restaurantId);
        return ControllerHelper.HandleApiResponse(response);
    }
}