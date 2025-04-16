using api.Features.User;

namespace api.Features.Auth.Interfaces;

public interface IJwtService
{
    Task<string> CreateTokenAsync(UserModel userModel);
    public string GenerateRefreshToken();
}