using api.Shared.DTOs.UserDto;

namespace api.Features.User;

public interface IUserService
{
    Task<List<UserDto>> GetAll();
    Task<UserDto?> Get(string userId);
    Task<UserDto?> PatchFullName(string userId, PatchFullNameDto patchFullNameDto);
    Task Ping(string username, string message);
}