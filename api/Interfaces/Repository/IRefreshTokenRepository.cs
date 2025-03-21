using api.Models;

namespace api.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task DeleteAsync(RefreshToken token);
    Task<List<RefreshToken>> GetByUserIdAsync(string userId);
    Task RemoveRangeAsync(List<RefreshToken> tokens);
}