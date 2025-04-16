using api.Features.Auth.Models;
using api.Shared.Auth.Enums;

namespace api.Features.Auth.Interfaces;

public interface IUserCredentialRepository
{
    Task<UserCredentialModel?> GetByUserIdAsync(string userId, CredentialType type);
    Task AddAsync(UserCredentialModel credentialModel);
    Task RemoveAsync(UserCredentialModel credentialModel);
    Task<List<UserCredentialModel>> GetByUserIdAsync(string userId);
    Task<UserCredentialModel?> UpdateAsync(UserCredentialModel credentialModel, params string[] updatedProperties);
    Task<UserCredentialModel?> GetByValueAsync(string hashedValue, CredentialType type);
}