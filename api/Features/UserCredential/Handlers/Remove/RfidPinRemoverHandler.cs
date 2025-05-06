using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Context.Remove;
using api.Features.UserCredential.Context.Remove.ExtraData;
using api.Features.UserCredential.Interfaces;
using api.Shared.Auth.Enums;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Remove;

public class RfidPinRemoverHandler : ICredentialRemoverHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly IPasswordHasher<UserModel> _passwordHasher;

    public RfidPinRemoverHandler(IUserCredentialRepository credentialRepository, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IPasswordHasher<UserModel> passwordHasher)
    {
        _credentialRepository = credentialRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _passwordHasher = passwordHasher;
    }

    public async Task RemoveAsync(RemoveCredentialContext context)
    {
        var userId = context.UserId;
        var mainPassword = context.MainPassword;
        string oldValue;

        if (context.ExtraData is RemoveRfidPinData data)
        {
            oldValue = data.OldValue;
        }
        else
        {
            throw new InvalidOperationException($"Expected RemoveCredentialData of type {nameof(RemoveRfidPinData)} but received {context.ExtraData?.GetType().Name ?? "null"}.");
        }

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

        var verifyOldValue = _passwordHasher.VerifyHashedPassword(userModel, credentialModel.HashedValue, oldValue);
        if (verifyOldValue == PasswordVerificationResult.Failed)
        {
            throw new Exception($"Invalid credential");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(userModel, mainPassword, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid login credential");
        }

        if (userModel.IsRfidPaymentEnabled)
        {
            userModel.IsRfidPaymentEnabled = false;
            await _userManager.UpdateAsync(userModel);
        }

        await _credentialRepository.RemoveAsync(credentialModel);
    }
}