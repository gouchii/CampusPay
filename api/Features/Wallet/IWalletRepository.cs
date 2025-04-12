namespace api.Features.Wallet;

public interface IWalletRepository
{
    Task<List<WalletModel>> GetAllAsync();
    Task<List<WalletModel>> GetAllByUserIdAsync(string userId);
    Task<WalletModel?> GetByUserIdAsync(string userId, WalletType type = WalletType.Default);
    Task<bool> UpdateBalanceAsync(WalletModel walletModel);
    Task<WalletModel?> CreateWalletAsync(string userId, WalletType type = WalletType.Default);
}