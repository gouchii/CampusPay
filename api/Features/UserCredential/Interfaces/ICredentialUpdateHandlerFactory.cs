using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialUpdateHandlerFactory
{
    public ICredentialUpdateHandler GetHandler(CredentialType type);
}