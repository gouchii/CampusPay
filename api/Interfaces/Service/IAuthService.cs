using api.DTOs.Account;

namespace api.Interfaces;

public interface IAuthService
{
    Task<NewUserDto> Register(string userName,string fullName, string email, string password);
    Task<NewUserDto?> LogIn(string userName, string password);
}