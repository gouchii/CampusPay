using api.Features.UserCredential.Context.Remove;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialRemoverHandler
{
    Task RemoveAsync(RemoveCredentialContext context);
}