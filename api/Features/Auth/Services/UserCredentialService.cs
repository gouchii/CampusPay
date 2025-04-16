using api.Features.Auth.Interfaces;
using api.Features.Auth.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using api.Shared.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace api.Features.Auth.Services;

public class UserCredentialService : IUserCredentialService
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly UserManager<UserModel> _userManager;

    public UserCredentialService(IUserCredentialRepository credentialRepository
        , IPasswordHasher<UserModel> passwordHasher, UserManager<UserModel> userManager)
    {
        _credentialRepository = credentialRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
    }


    public async Task<bool> VerifyCredentialAsync(string userId, string value, CredentialType type)
    {
        var userModel = await _userManager.FindByIdAsync(userId);

        if (userModel == null)
        {
            throw new Exception($"User not found");
        }

        var credentialModel = await _credentialRepository.GetByTypeAsync(userId, type);
        if (credentialModel == null)
        {
            throw new Exception($"User does not have a {type} registered yet");
        }

        var result = _passwordHasher.VerifyHashedPassword(userModel, credentialModel.HashedValue, value);

        credentialModel.LastUsedAt = DateTime.Now;

        await _credentialRepository.UpdateAsync(credentialModel, [
            nameof(UserCredentialModel.LastUsedAt)
        ]);

        return result == PasswordVerificationResult.Success;
    }

    public async Task RegisterCredentialAsync(string userId, string value, CredentialType type)
    {
        var userModel = await _userManager.FindByIdAsync(userId);

        if (userModel == null)
        {
            throw new Exception($"User not found");
        }

        var hashedValue = _passwordHasher.HashPassword(userModel, value);
        var credentialModel = new UserCredentialModel()
        {
            UserId = userId,
            HashedValue = hashedValue,
            Type = type
        };
        await _credentialRepository.AddAsync(credentialModel);
    }

    public Task<bool> RemoveCredentialAsync(string userId, string value, CredentialType type)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateCredentialAsync(string userId, string value, CredentialType type)
    {
        throw new NotImplementedException();
    }
}

// public class UserCredentialService : IUserCredentialService
// {
//     private readonly AppDbContext _context;
//     private readonly IPasswordHasher<UserModel> _hasher;
//
//     public UserCredentialService(AppDbContext context, IPasswordHasher<UserModel> hasher)
//     {
//         _context = context;
//         _hasher = hasher;
//     }
//
//     public async Task<bool> VerifyPinAsync(string userId, string pin)
//     {
//         var credential = await _context.UserCredentials
//             .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.Type == CredentialType.RfidPin);
//
//         if (credential == null) return false;
//
//         var user = await _context.Users.FindAsync(userId);
//         var result = _hasher.VerifyHashedPassword(user, credential.HashedValue, pin);
//
//         return result == PasswordVerificationResult.Success;
//     }
// }