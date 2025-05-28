using api.Features.UserCredential.Context.Update;

namespace api.Features.UserCredential.Interfaces;

public interface ICredentialUpdateHandler
{
    Task UpdateAsync(UpdateCredentialContext context);
}