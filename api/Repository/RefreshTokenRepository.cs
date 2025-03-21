using api.Data;
using api.Interfaces.Repository;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Service.Authorization;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    //todo refactor this class

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<List<RefreshToken>> GetByUserIdAsync(string userId)
    {
        return await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken token)
    {
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<RefreshToken> tokens)
    {
        _context.RefreshTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }


}