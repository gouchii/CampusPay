using api.Features.UserCredential.Models;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialFactory
{
    Task<UserCredentialModel> CreateAsync(string userId, string hashedValue, CredentialType type);
}