using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Handlers.Find;

public class CredentialFinderHandler : ICredentialFinderHandler
{
    private readonly IUserCredentialRepository _credentialRepository;

    public CredentialFinderHandler(IUserCredentialRepository credentialRepository)
    {
        _credentialRepository = credentialRepository;
    }

    public Task<UserCredentialModel?> GetByUserIdAsync(string userId, CredentialType type)
    {
        return _credentialRepository.GetByUserIdAsync(userId, type);
    }
}