using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialRegistrationHandlerFactory
{
    ICredentialRegistrationHandler GetHandler(CredentialType type);
}