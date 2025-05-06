namespace api.Features.UserCredential.Interfaces;

public interface ICredentialRegistrationHandler
{
    Task RegisterAsync(string userId, string value);
}