using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Interfaces;
using api.Shared.Auth.Enums;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Register;

public class RfidPinRegistrationHandler : ICredentialRegistrationHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly ICredentialFactory _credentialFactory;
    private readonly IPasswordHasher<UserModel> _passwordHasher;

    public RfidPinRegistrationHandler(ICredentialFactory credentialFactory,
        UserManager<UserModel> userManager, IUserCredentialRepository credentialRepository,
        IPasswordHasher<UserModel> passwordHasher)
    {
        _credentialFactory = credentialFactory;
        _userManager = userManager;
        _credentialRepository = credentialRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task RegisterAsync(string userId, string value)
    {
        const CredentialType type = CredentialType.RfidPin;

        var userModel = await _userManager.FindByIdAsync(userId);

        if (userModel == null)
        {
            throw new Exception($"User not found");
        }

        var credentialModel = await _credentialRepository.GetByUserIdAsync(userId, type);
        if (credentialModel != null)
        {
            throw new Exception($"User already has {type} registered");
        }

        var hashedValue = _passwordHasher.HashPassword(userModel, value);

        await _credentialFactory.CreateAsync(userId, hashedValue, type);
    }
}