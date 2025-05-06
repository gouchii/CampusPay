using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialRemoverHandlerFactory
{
    public ICredentialRemoverHandler GetHandler(CredentialType type);
}