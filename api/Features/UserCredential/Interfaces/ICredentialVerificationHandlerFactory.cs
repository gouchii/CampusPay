using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialVerificationHandlerFactory
{
    ICredentialVerificationHandler GetHandler(CredentialType type);
}