using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Enums.UserCredential;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Verify;

public class RfidPinVerificationHandler : ICredentialVerificationHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly UserManager<UserModel> _userManager;

    public RfidPinVerificationHandler(IUserCredentialRepository credentialRepository,
        IPasswordHasher<UserModel> passwordHasher, UserManager<UserModel> userManager)
    {
        _credentialRepository = credentialRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
    }

    public async Task<bool> VerifyAsync(string userId, string value)
    {
        const CredentialType type = CredentialType.RfidPin;

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


        if (result == PasswordVerificationResult.Failed)
        {
            return false;
        }

        credentialModel.LastUsedAt = DateTime.Now;
        await _credentialRepository.UpdateAsync(credentialModel, [
            nameof(UserCredentialModel.LastUsedAt)
        ]);

        return true;
    }
}