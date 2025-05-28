using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Enums.UserCredential;
using api.Shared.Helpers;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Verify;

public class RfidTagVerificationHandler : ICredentialVerificationHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly IConfiguration _configuration;

    public RfidTagVerificationHandler(IConfiguration configuration,
        UserManager<UserModel> userManager, IUserCredentialRepository credentialRepository)
    {
        _configuration = configuration;
        _userManager = userManager;
        _credentialRepository = credentialRepository;
    }

    public async Task<bool> VerifyAsync(string userId, string value)
    {
        const CredentialType type = CredentialType.RfidTag;
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

        var signingKey = _configuration["UserCredentials:RfidTagSigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("RfidTag signing key is missing from configuration.");
        }

        var result = TokenHasher.HashToken(value, signingKey);
        if (result != credentialModel.HashedValue)
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