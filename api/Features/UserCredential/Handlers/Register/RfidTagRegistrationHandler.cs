using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Interfaces;
using api.Shared.Auth.Enums;
using api.Shared.Helpers;
using Microsoft.AspNetCore.Identity;

namespace api.Features.UserCredential.Handlers.Register;

public class RfidTagRegistrationHandler : ICredentialRegistrationHandler
{
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ICredentialFactory _credentialFactory;


    public RfidTagRegistrationHandler(IUserCredentialRepository credentialRepository,
        UserManager<UserModel> userManager, IConfiguration configuration,
        ICredentialFactory credentialFactory)
    {
        _credentialRepository = credentialRepository;
        _userManager = userManager;
        _configuration = configuration;
        _credentialFactory = credentialFactory;
    }

    public async Task RegisterAsync(string userId, string value)
    {
        const CredentialType type = CredentialType.RfidTag;
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

        //rfid tag is hashed differently than other credentials
        var signingKey = _configuration["UserCredentials:RfidTagSigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("RfidTag signing key is missing from configuration.");
        }

        var hashedValue = TokenHasher.HashToken(value, signingKey);

        var existingCredential = await _credentialRepository.GetByValueAsync(hashedValue, type);

        if (existingCredential != null)
        {
            throw new Exception("Invalid RfidTag Value");
        }

        await _credentialFactory.CreateAsync(userId, hashedValue, type);
    }
}