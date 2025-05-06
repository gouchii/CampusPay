using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Context.Update.Interfaces;

public interface IUpdateCredentialContextBuilderFactory
{
    IUpdateCredentialContextBuilder GetBuilder(CredentialType type);
}