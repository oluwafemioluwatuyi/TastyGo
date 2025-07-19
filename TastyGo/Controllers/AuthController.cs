using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyGo.DTOs;
using TastyGo.Helpers;
using TastyGo.Interfaces.Services;

namespace TastyGo.Controllers;

[ApiController]
[Route("api/auth")]

public class AuthController : ControllerBase

{
    private readonly IAuthService authService;
    public AuthController(IAuthService authService)
    {
        this.authService = authService;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var response = await authService.RegisterAsync(registerRequestDto);
        return ControllerHelper.HandleApiResponse(response);

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginDto)
    {
        var response = await authService.LoginAsync(loginDto);
        return ControllerHelper.HandleApiResponse(response);

    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var response = await authService.RefreshTokenAsync(refreshTokenRequestDto.ExistingToken);
        return ControllerHelper.HandleApiResponse(response);

    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDto verifyEmailDto)
    {
        var response = await authService.VerifyEmail(verifyEmailDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
    {
        var response = await authService.ForgotPasswordAsync(forgotPasswordRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
    {
        var response = await authService.ResetPasswordAsync(resetPasswordDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost]
    [Route("create-pin")]
    [Authorize]
    public async Task<IActionResult> CreatePin([FromBody] CreatePinRequestDto createPinRequestDtoPasswordDto)
    {
        var response = await authService.CreatePin(createPinRequestDtoPasswordDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
    [Route("change-pin")]
    [Authorize]
    public async Task<IActionResult> ResetPin([FromBody] ResetPinRequestDto resetPinDto)
    {
        var response = await authService.ResetPin(resetPinDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
    [Route("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
    {
        var response = await authService.ResetPasswordAsync(resetPasswordDto);
        return ControllerHelper.HandleApiResponse(response);
    }



}