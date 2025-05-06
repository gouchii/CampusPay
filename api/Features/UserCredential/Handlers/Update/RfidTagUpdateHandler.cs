using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Context.Update;
using api.Features.UserCredential.Context.Update.ExtraData;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Auth.Enums;
using api.Shared.Helpers;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Update;

public class RfidTagUpdateHandler : ICredentialUpdateHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<UserModel> _signInManager;

    public RfidTagUpdateHandler(UserManager<UserModel> userManager,
        IUserCredentialRepository credentialRepository, IConfiguration configuration, SignInManager<UserModel> signInManager)
    {
        _userManager = userManager;
        _credentialRepository = credentialRepository;
        _configuration = configuration;
        _signInManager = signInManager;
    }

    public async Task UpdateAsync(UpdateCredentialContext context)
    {
        var userId = context.UserId;
        var mainPassword = context.MainPassword;
        var newValue = context.NewValue;

        if (context.ExtraData is UpdateRfidTagData)
        {
        }
        else
        {
            throw new InvalidOperationException($"Expected UpdateCredentialData of type {nameof(UpdateRfidTagData)} but received {context.ExtraData?.GetType().Name ?? "null"}.");
        }

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

        var result = await _signInManager.CheckPasswordSignInAsync(userModel, mainPassword, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid credential");
        }

        var hashedNewValue = TokenHasher.HashToken(newValue, signingKey);

        var existingCredential = await _credentialRepository.GetByValueAsync(hashedNewValue, CredentialType.RfidTag);
        if (existingCredential != null)
        {
            throw new Exception("Invalid RfidTag Value");
        }

        credentialModel.HashedValue = hashedNewValue;

        await _credentialRepository.UpdateAsync(credentialModel, [
            nameof(UserCredentialModel.HashedValue)
        ]);
    }
}