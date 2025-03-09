using api.Models;

namespace api.Interfaces;

public interface IWalletRepository
{
    Task<List<Wallet>> GetAllAsync();
    Task<List<Wallet>> GetAllByUserIdAsync(int userId);
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<bool> UpdateBalanceAsync(Wallet wallet);
}