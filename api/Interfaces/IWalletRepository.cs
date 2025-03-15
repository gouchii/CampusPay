using api.Models;

namespace api.Interfaces;

public interface IWalletRepository
{
    Task<List<Wallet>> GetAllAsync();
    Task<List<Wallet>> GetAllByUserIdAsync(string userId);
    Task<Wallet?> GetByUserIdAsync(string userId);
    Task<bool> UpdateBalanceAsync(Wallet wallet);
}