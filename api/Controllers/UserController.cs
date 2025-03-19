using api.DTOs.Account;
using api.DTOs.User;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;

    public UserController(IUserService userService, UserManager<User> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userManager.Users.Include(u => u.Wallets).ToListAsync();

        var userDto = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Add(user.ToUserDto(roles.FirstOrDefault() ?? "No Role"));
        }

        return Ok(userDto);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get()
    {
        var users = await _userManager.Users.Include(u => u.Wallets).ToListAsync();

        var userDto = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Add(user.ToUserDto(roles.FirstOrDefault() ?? "No Role"));
        }

        return Ok(userDto);
    }

}