using api.Models;

namespace api.Interfaces.Service;

public interface IJwtService
{
    Task<string> CreateTokenAsync(User user);
    public string GenerateRefreshToken();
}