using api.Shared.DTOs.UserCredential;
using api.Shared.DTOs.UserCredential.Validate;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialValidatorHandler
{
    Task<ValidateCredentialResultDto> ValidateAsync(string value);
}