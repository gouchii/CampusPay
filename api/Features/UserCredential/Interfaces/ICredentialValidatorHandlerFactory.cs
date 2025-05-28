using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialValidatorHandlerFactory
{
    public ICredentialValidatorHandler GetHandler(CredentialType type);
}