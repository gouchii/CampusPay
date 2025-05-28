using api.Features.SignalR;
using api.Shared.DTOs.UserDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace api.Features.User;

public class UserService : IUserService
{
    private readonly UserManager<UserModel> _userManager;
    private readonly IHubContext<UserHub> _hubContext;

    public UserService(UserManager<UserModel> userManager, IHubContext<UserHub> hubContext)
    {
        _userManager = userManager;
        _hubContext = hubContext;
    }


    public async Task<List<UserDto>> GetAll()
    {
        var users = await _userManager.Users.Include(u => u.Wallets)
            .Include(u => u.SentTransactions)
            .Include(u => u.ReceivedTransactions)
            .ToListAsync();

        var userDto = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Add(user.ToUserDto(roles.FirstOrDefault() ?? "No Role"));
        }

        return userDto;
    }


    public async Task<UserDto?> Get(string userId)
    {
        if (userId == null)
        {
            throw new ArgumentException("User ID must not be empty.", nameof(userId));
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID '{userId}' not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);


        return user.ToUserDto(roles.FirstOrDefault() ?? "No Role");
    }

    public async Task<UserDto?> PatchFullName(string userId, PatchFullNameDto patchFullNameDto)
    {
        if (userId == null)
        {
            throw new ArgumentException("User ID must not be empty.", nameof(userId));
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID '{userId}' not found.");
        }

        user.FullName = patchFullNameDto.FullName;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update user: {errors}");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return user.ToUserDto(roles.FirstOrDefault() ?? "No Role");
    }

    public async Task Ping(string username, string fromUserId)
    {
        var user = await _userManager.FindByNameAsync(username);
        await _hubContext.Clients.User(user?.Id ?? throw new InvalidOperationException()).SendAsync("ReceivePing", fromUserId);
    }
}