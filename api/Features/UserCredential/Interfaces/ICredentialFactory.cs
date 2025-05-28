using api.Features.UserCredential.Models;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialFactory
{
    Task<UserCredentialModel> CreateAsync(string userId, string hashedValue, CredentialType type);
}