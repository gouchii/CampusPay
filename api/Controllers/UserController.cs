using api.DTOs.User;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public UserController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userRepo.GetAllAsync();
        return Ok(users.Select(u => u.ToUserDto()).ToList());
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var user = await _userRepo.GetByIdAsync(id);

        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userCreateDto)
    {
        var userModel = userCreateDto.ToUserFromCreateDto();

        await _userRepo.CreateAsync(userModel);
        return CreatedAtAction(nameof(GetById), new { id = userModel.Id }, userModel);
    }

    [HttpPut]
    [Route("Update/{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userUpdateDto)
    {
        var user = await _userRepo.UpdateAsync(id, userUpdateDto.ToUserFromUpdateDto());
        if (user == null) return NotFound("User not found");

        return Ok(user.ToUserDto());
    }

    [HttpDelete]
    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var userModel = await _userRepo.DeleteAsync(id);
        if (userModel == null) return NotFound("User does not exits");

        return Ok(userModel);
    }
}