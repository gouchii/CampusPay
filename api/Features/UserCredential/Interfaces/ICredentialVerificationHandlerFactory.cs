using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialVerificationHandlerFactory
{
    ICredentialVerificationHandler GetHandler(CredentialType type);
}