using api.Features.UserCredential.Models;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface IUserCredentialRepository
{
    Task<UserCredentialModel?> GetByUserIdAsync(string userId, CredentialType type);
    Task AddAsync(UserCredentialModel credentialModel);
    Task RemoveAsync(UserCredentialModel credentialModel);
    Task<List<UserCredentialModel>> GetByUserIdAsync(string userId);
    Task<UserCredentialModel?> UpdateAsync(UserCredentialModel credentialModel, params string[] updatedProperties);
    Task<UserCredentialModel?> GetByValueAsync(string hashedValue, CredentialType type);
}