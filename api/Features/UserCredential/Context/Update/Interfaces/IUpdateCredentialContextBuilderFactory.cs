using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Context.Update.Interfaces;

public interface IUpdateCredentialContextBuilderFactory
{
    IUpdateCredentialContextBuilder GetBuilder(CredentialType type);
}