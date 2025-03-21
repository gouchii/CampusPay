using api.DTOs.Account;

namespace api.Interfaces.Service;

public interface IAuthService
{
    Task<NewUserDto> Register(string userName,string fullName, string email, string password);
    Task<NewUserDto?> LogIn(string userName, string password);
    Task<NewUserDto?> RefreshJwtToken(string refreshToken, string userId);
}