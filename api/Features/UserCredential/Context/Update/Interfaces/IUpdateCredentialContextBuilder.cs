using api.Shared.DTOs.UserCredential;
using api.Shared.DTOs.UserCredential.Update;

namespace api.Features.UserCredential.Context.Update.Interfaces;

public interface IUpdateCredentialContextBuilder
{
    UpdateCredentialContext Build(BaseUpdateCredentialRequestDto request, string userId);
}