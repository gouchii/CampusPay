using api.Features.Auth.Interfaces;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Factories;

public class CredentialFactory : ICredentialFactory
{
    private readonly IUserCredentialRepository _credentialRepository;

    public CredentialFactory(IUserCredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    //separating this is kinda unnecessary I think but meh
    //this should not be called by itself in the service layer, use handlers instead
    public async Task<UserCredentialModel> CreateAsync(string userId, string hashedValue, CredentialType type)
    {
        var credentialModel = new UserCredentialModel()
        {
            UserId = userId,
            HashedValue = hashedValue,
            Type = type
        };
        await _credentialRepository.AddAsync(credentialModel);
        return credentialModel;
    }
}