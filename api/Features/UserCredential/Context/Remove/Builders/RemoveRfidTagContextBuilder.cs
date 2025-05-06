using api.Features.UserCredential.Context.Remove.ExtraData;
using api.Features.UserCredential.Context.Remove.Interfaces;
using api.Shared.DTOs.UserCredential.Remove;

namespace api.Features.UserCredential.Context.Remove.Builders;

public class RemoveRfidTagContextBuilder : IRemoveCredentialContextBuilder
{
    public RemoveCredentialContext Build(BaseRemoveCredentialRequestDto request, string userId)
    {
        string mainPassword;
        if (request is RemoveRfidTagRequestDto requestData)
        {
            mainPassword = requestData.MainPassword;
        }
        else
        {
            throw new InvalidOperationException($"Expected RemoveCredentialRequestDto of type {nameof(RemoveRfidTagRequestDto)} but received {request.GetType().Name}.");
        }

        return new RemoveCredentialContext
        {
            UserId = userId,
            MainPassword = mainPassword,
            ExtraData = new RemoveRfidTagData()
        };
    }
}