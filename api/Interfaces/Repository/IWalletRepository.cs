using api.Enums;
using api.Models;

namespace api.Interfaces.Repository;

public interface IWalletRepository
{
    Task<List<Wallet>> GetAllAsync();
    Task<List<Wallet>> GetAllByUserIdAsync(string userId);
    Task<Wallet?> GetByUserIdAsync(string userId,WalletType type= WalletType.Default);
    Task<bool> UpdateBalanceAsync(Wallet wallet);
    Task<Wallet?>  CreateWalletAsync(string userId, WalletType type= WalletType.Default);
}