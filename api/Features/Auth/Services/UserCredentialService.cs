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

        var credentialModel = await _credentialRepository.GetByUserIdAsync(userId, type);
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

        if (type == CredentialType.RfidTag)
        {
            var existingCredential = await _credentialRepository.GetByValueAsync(hashedValue, CredentialType.RfidTag);
            if (existingCredential != null)
            {
                throw new Exception("Invalid RfidTag Value");
            }
        }

        var credentialModel = new UserCredentialModel()
        {
            UserId = userId,
            HashedValue = hashedValue,
            Type = type
        };
        await _credentialRepository.AddAsync(credentialModel);
    }

    public async Task RemoveCredentialAsync(string userId, string value, CredentialType type)
    {
        var userModel = await _userManager.FindByIdAsync(userId);

        if (userModel == null)
        {
            throw new Exception($"User not found");
        }

        var credentialModel = await _credentialRepository.GetByUserIdAsync(userId, type);
        if (credentialModel == null)
        {
            throw new Exception($"User does not have a {type} registered yet");
        }

        var verifyValue = _passwordHasher.VerifyHashedPassword(userModel, credentialModel.HashedValue, value);
        if (verifyValue == PasswordVerificationResult.Failed)
        {
            throw new Exception($"Invalid credential");
        }

        await _credentialRepository.RemoveAsync(credentialModel);
    }

    public async Task UpdateCredentialAsync(string userId, string oldValue, string newValue, CredentialType type)
    {
        var userModel = await _userManager.FindByIdAsync(userId);

        if (userModel == null)
        {
            throw new Exception($"User not found");
        }

        var credentialModel = await _credentialRepository.GetByUserIdAsync(userId, type);
        if (credentialModel == null)
        {
            throw new Exception($"User does not have a {type} registered yet");
        }

        var verifyOldValue = _passwordHasher.VerifyHashedPassword(userModel, credentialModel.HashedValue, oldValue);
        if (verifyOldValue == PasswordVerificationResult.Failed)
        {
            throw new Exception($"Invalid credential");
        }

        var hashedNewValue = _passwordHasher.HashPassword(userModel, newValue);
        if (type == CredentialType.RfidTag)
        {
            var existingCredential = await _credentialRepository.GetByValueAsync(hashedNewValue, CredentialType.RfidTag);
            if (existingCredential != null)
            {
                throw new Exception("Invalid RfidTag Value");
            }
        }

        credentialModel.HashedValue = hashedNewValue;
        await _credentialRepository.UpdateAsync(credentialModel, [
            nameof(UserCredentialModel.HashedValue)
        ]);
    }
}