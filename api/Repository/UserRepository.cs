using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.Include(u => u.Wallets).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.Include(u => u.Wallets).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User userModel)
    {
        var defaultWallet = new Wallet
        {
            Balance = 0.0m
        };

        userModel.Wallets.Add(defaultWallet);
        await _context.Users.AddAsync(userModel);
        await _context.SaveChangesAsync();
        return userModel;
    }

    public async Task<User?> DeleteAsync(int id)
    {
        var userModel = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (userModel == null) return null;

        _context.Users.Remove(userModel);
        await _context.SaveChangesAsync();
        return userModel;
    }

    public async Task<User?> UpdateAsync(int id, User userModel)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null) return null;

        existingUser.Name = userModel.Name;
        existingUser.EmailAddress = userModel.EmailAddress;

        await _context.SaveChangesAsync();
        return existingUser;
    }
}