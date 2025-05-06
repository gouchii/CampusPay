using api.Features.UserCredential.Models;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialFinderHandler
{
    Task<UserCredentialModel?> GetByUserIdAsync(string userId, CredentialType type);
}