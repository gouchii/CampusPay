using api.Shared.Auth.Enums;

namespace api.Shared.Auth.Interfaces;

public interface IUserCredentialService
{
    Task<bool> VerifyCredentialAsync(string userId, string value, CredentialType type);
    Task RegisterCredentialAsync(string userId, string value, CredentialType type);
    Task RemoveCredentialAsync(string userId, string value, CredentialType type);
    Task UpdateCredentialAsync(string userId, string oldValue, string newValue, CredentialType type);
}