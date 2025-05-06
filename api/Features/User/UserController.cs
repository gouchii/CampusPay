using api.Shared.DTOs.UserDto;
using api.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Features.User;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
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

            var userDtos = await _userService.GetAll();
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("")]
    public async Task<IActionResult> Get()
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

            var userDtos = await _userService.Get(userId);
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("full-name")]
    public async Task<IActionResult> PatchFullname([FromBody] PatchFullNameDto request)
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

            var userDtos = await _userService.PatchFullName(userId, request);
            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}