namespace api.Features.UserCredential.Interfaces;

public interface ICredentialVerificationHandler
{
    Task<bool> VerifyAsync(string userId, string value);
}