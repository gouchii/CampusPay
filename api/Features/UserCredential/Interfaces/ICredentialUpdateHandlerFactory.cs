using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialUpdateHandlerFactory
{
    public ICredentialUpdateHandler GetHandler(CredentialType type);
}