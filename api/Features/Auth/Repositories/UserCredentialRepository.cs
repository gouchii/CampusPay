using api.Data;
using api.Features.Auth.Interfaces;
using api.Features.Auth.Models;
using api.Shared.Auth.Enums;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Auth.Repositories;

public class UserCredentialRepository : IUserCredentialRepository
{
    private readonly AppDbContext _context;

    public UserCredentialRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserCredentialModel?> GetByUserIdAsync(string userId, CredentialType type)
    {
        return await _context.UserCredentials.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.Type == type);
    }

    public async Task AddAsync(UserCredentialModel credentialModel)
    {
        var duplicate = await GetByUserIdAsync(credentialModel.UserId, credentialModel.Type);

        if (duplicate != null)
        {
            throw new Exception($"Duplicate credential type is not allowed");
        }

        await _context.UserCredentials.AddAsync(credentialModel);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(UserCredentialModel credentialModel)
    {
        _context.UserCredentials.Remove(credentialModel);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserCredentialModel>> GetByUserIdAsync(string userId)
    {
        return await _context.UserCredentials.Where(uc => uc.UserId == userId).ToListAsync();
    }

    public async Task<UserCredentialModel?> UpdateAsync(UserCredentialModel credentialModel, params string[] updatedProperties)
    {
        var existingUserCredentialModel = await _context.UserCredentials.FindAsync(credentialModel.Id);
        if (existingUserCredentialModel == null)
        {
            return null;
        }

        foreach (var property in updatedProperties)
        {
            switch (property)
            {
                case nameof(UserCredentialModel.LastUsedAt):
                    existingUserCredentialModel.LastUsedAt = credentialModel.LastUsedAt;
                    break;
                case nameof(UserCredentialModel.HashedValue):
                    existingUserCredentialModel.HashedValue = credentialModel.HashedValue;
                    break;
            }
        }

        await _context.SaveChangesAsync();
        return existingUserCredentialModel;
    }

    public async Task<UserCredentialModel?> GetByValueAsync(string hashedValue, CredentialType type)
    {
        return await _context.UserCredentials.FirstOrDefaultAsync(uc => uc.HashedValue == hashedValue && uc.Type == type);
    }
}