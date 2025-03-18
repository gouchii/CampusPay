using api.Data;
using api.Enums;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Wallet>> GetAllAsync()
    {
        return await _context.Wallets.ToListAsync();
    }

    public async Task<List<Wallet>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Wallets.Where(w => w.UserId == userId).ToListAsync();
    }

    public async Task<Wallet?> GetByUserIdAsync(string userId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<bool> UpdateBalanceAsync(Wallet walletModel)
    {
        var existingWallet = await _context.Wallets.FindAsync(walletModel.Id);
        if (existingWallet == null)
        {
            return false; // Wallet not found, update failed
        }

        existingWallet.Balance = walletModel.Balance;
        return await _context.SaveChangesAsync() > 0;
    }

    //todo redo this
    public async Task<Wallet?> CreateWalletAsync(string userId, WalletType type = WalletType.Default)
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

        var wallet = new Wallet
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