using api.Features.Auth.Models;

namespace api.Features.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel?> GetByTokenAsync(string token);
    Task AddAsync(RefreshTokenModel tokenModel);
    Task UpdateAsync(RefreshTokenModel tokenModel);
    Task DeleteAsync(RefreshTokenModel tokenModel);
    Task<List<RefreshTokenModel>> GetByUserIdAsync(string userId);
    Task RemoveRangeAsync(List<RefreshTokenModel> tokens);
}