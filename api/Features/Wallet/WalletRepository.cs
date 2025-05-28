using api.Data;
using api.Shared.Enums.Wallet;
using api.Shared.Interfaces.Wallet;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Wallet;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WalletModel>> GetAllAsync()
    {
        return await _context.Wallets.ToListAsync();
    }

    public async Task<List<WalletModel>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Wallets.Where(w => w.UserId == userId).ToListAsync();
    }

    public async Task<WalletModel?> GetByUserIdAsync(string userId, WalletType type = WalletType.Default)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId && w.Type == type);
    }

    public async Task<bool> UpdateBalanceAsync(WalletModel walletModelModel)
    {
        var existingWallet = await _context.Wallets.FindAsync(walletModelModel.Id);
        if (existingWallet == null)
        {
            return false;
        }

        existingWallet.Balance = walletModelModel.Balance;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<WalletModel?> CreateWalletAsync(string userId, WalletType type = WalletType.Default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }

        var existingWallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId && w.Type == type);

        if (existingWallet != null)
        {
            throw new Exception($"User already has a wallet of type {type}");
        }

        var wallet = new WalletModel
        {
            UserId = userId,
            Type = type,
            Balance = 0.0m,
        };
        await _context.Wallets.AddAsync(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }
}