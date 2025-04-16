using api.Shared.DTOs.Authentication;

namespace api.Shared.Auth.Interfaces;

public interface IAuthService
{
    Task<NewUserDto> Register(string userName, string fullName, string email, string password);
    Task<NewUserDto?> LogIn(string userName, string password);
    Task<NewUserDto?> RefreshJwtToken(string refreshToken, string userId);
    Task Logout(string userId);
}