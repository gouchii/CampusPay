using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialValidatorHandlerFactory
{
    public ICredentialValidatorHandler GetHandler(CredentialType type);
}