namespace api.Features.Auth.Interface;

public interface IJwtService
{
    Task<string> CreateTokenAsync(Features.User.UserModel userModel);
    public string GenerateRefreshToken();
}