using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Context.Update;
using api.Features.UserCredential.Context.Update.ExtraData;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Auth.Enums;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Update;

public class RfidPinUpdateHandler : ICredentialUpdateHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly SignInManager<UserModel> _signInManager;

    public RfidPinUpdateHandler(IUserCredentialRepository credentialRepository, UserManager<UserModel> userManager, IPasswordHasher<UserModel> passwordHasher, SignInManager<UserModel> signInManager)
    {
        _credentialRepository = credentialRepository;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _signInManager = signInManager;
    }

    public async Task UpdateAsync(UpdateCredentialContext context)
    {
        var userId = context.UserId;
        var mainPassword = context.MainPassword;
        var newValue = context.NewValue;
        string oldValue;

        if (context.ExtraData is UpdateRfidPinData data)
        {
            oldValue = data.OldValue;
        }
        else
        {
            throw new InvalidOperationException($"Expected UpdateCredentialData of type {nameof(UpdateRfidPinData)} but received {context.ExtraData?.GetType().Name ?? "null"}.");
        }


        const CredentialType type = CredentialType.RfidPin;

        var userModel = await _userManager.FindByIdAsync(userId) ?? throw new Exception($"User not found");


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

        var result = await _signInManager.CheckPasswordSignInAsync(userModel, mainPassword, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid credential");
        }


        var hashedNewValue = _passwordHasher.HashPassword(userModel, newValue);

        credentialModel.HashedValue = hashedNewValue;

        await _credentialRepository.UpdateAsync(credentialModel, [
            nameof(UserCredentialModel.HashedValue)
        ]);
    }
}