using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Context.Remove;
using api.Features.UserCredential.Context.Remove.ExtraData;
using api.Features.UserCredential.Interfaces;
using api.Shared.Enums.UserCredential;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Remove;

public class RfidTagRemoverHandler : ICredentialRemoverHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;

    public RfidTagRemoverHandler(IUserCredentialRepository credentialRepository, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
    {
        _credentialRepository = credentialRepository;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task RemoveAsync(RemoveCredentialContext context)
    {
        var userId = context.UserId;
        var mainPassword = context.MainPassword;

        if (context.ExtraData is RemoveRfidTagData)
        {
        }
        else
        {
            throw new InvalidOperationException($"Expected RemoveCredentialData of type {nameof(RemoveRfidTagData)} but received {context.ExtraData?.GetType().Name ?? "null"}.");
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

        var result = await _signInManager.CheckPasswordSignInAsync(userModel, mainPassword, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid login credentials.");
        }

        if (userModel.IsRfidPaymentEnabled)
        {
            userModel.IsRfidPaymentEnabled = false;
            await _userManager.UpdateAsync(userModel);
        }

        await _credentialRepository.RemoveAsync(credentialModel);
    }
}