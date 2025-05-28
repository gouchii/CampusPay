using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialRegistrationHandlerFactory
{
    ICredentialRegistrationHandler GetHandler(CredentialType type);
}