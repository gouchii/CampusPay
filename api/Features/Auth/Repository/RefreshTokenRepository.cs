using api.Data;
using api.Features.Auth.Interface;
using api.Features.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Auth.Repository;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    //todo refactor this class

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshTokenModel?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<List<RefreshTokenModel>> GetByUserIdAsync(string userId)
    {
        return await _context.Set<RefreshTokenModel>()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(RefreshTokenModel tokenModel)
    {
        await _context.RefreshTokens.AddAsync(tokenModel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshTokenModel tokenModel)
    {
        _context.RefreshTokens.Update(tokenModel);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshTokenModel tokenModel)
    {
        _context.RefreshTokens.Remove(tokenModel);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<RefreshTokenModel> tokens)
    {
        _context.RefreshTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }


}