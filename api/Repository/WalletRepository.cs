using api.Data;
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

    public async Task<List<Wallet>> GetAllByUserIdAsync(int userId)
    {
        return await _context.Wallets.Where(w => w.UserId == userId).ToListAsync();
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
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
}