using api.Features.UserCredential.Context.Remove;
using api.Features.UserCredential.Context.Update;
using api.Features.UserCredential.Models;
using api.Shared.DTOs.UserCredential.Validate;
using api.Shared.Enums.UserCredential;

namespace api.Shared.UserCredential.Interfaces;

public interface IUserCredentialService
{
    Task<bool> VerifyCredentialAsync(string userId, string value, CredentialType type);
    Task RegisterCredentialAsync(string userId, string value, CredentialType type);
    Task RemoveCredentialAsync(RemoveCredentialContext context, CredentialType type);
    Task UpdateCredentialAsync(UpdateCredentialContext context, CredentialType type);
    Task<ValidateCredentialResultDto> ValidateCredentialAsync(string value, CredentialType type);
    Task<UserCredentialModel?> FindCredentialAsync(string userId, CredentialType type);
}