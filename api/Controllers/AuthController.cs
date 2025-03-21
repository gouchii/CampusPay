using api.DTOs.Account;
using api.Extensions;
using api.Interfaces;
using api.Interfaces.Service;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/account")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDto = await _authService.Register(registerDto.UserName, registerDto.FullName,
                registerDto.Email, registerDto.Password);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogOut()
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            await _authService.Logout(userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] LoginRequestDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userDto = await _authService.LogIn(loginDto.Username, loginDto.Password);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("refreshTokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokenRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            var userDto = await _authService.RefreshJwtToken(requestDto.RefreshToken, userId);
            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}