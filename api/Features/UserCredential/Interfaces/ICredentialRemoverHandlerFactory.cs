using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialRemoverHandlerFactory
{
    public ICredentialRemoverHandler GetHandler(CredentialType type);
}