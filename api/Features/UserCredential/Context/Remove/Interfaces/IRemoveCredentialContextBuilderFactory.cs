using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Context.Remove.Interfaces;

public interface IRemoveCredentialContextBuilderFactory
{
    IRemoveCredentialContextBuilder GetBuilder(CredentialType type);
}