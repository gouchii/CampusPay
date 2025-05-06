using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Context.Remove.Interfaces;

public interface IRemoveCredentialContextBuilderFactory
{
    IRemoveCredentialContextBuilder GetBuilder(CredentialType type);
}