using api.Shared.DTOs.UserCredential.Remove;

namespace api.Features.UserCredential.Context.Remove.Interfaces;

public interface IRemoveCredentialContextBuilder
{
    RemoveCredentialContext Build(BaseRemoveCredentialRequestDto request, string userId);
}