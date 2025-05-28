using api.Features.UserCredential.Context.Remove.ExtraData;
using api.Features.UserCredential.Context.Remove.Interfaces;
using api.Shared.DTOs.UserCredential.Remove;

namespace api.Features.UserCredential.Context.Remove.Builders;

public class RemoveRfidPinContextBuilder : IRemoveCredentialContextBuilder
{
    public RemoveCredentialContext Build(BaseRemoveCredentialRequestDto request, string userId)
    {
        string mainPassword;
        string oldValue;
        if (request is RemoveRfidPinRequestDto requestData)
        {
            mainPassword = requestData.MainPassword;
            oldValue = requestData.OldValue;
        }
        else
        {
            throw new InvalidOperationException($"Expected RemoveCredentialRequestDto of type {nameof(RemoveRfidPinRequestDto)} but received {request.GetType().Name}.");
        }

        return new RemoveCredentialContext
        {
            UserId = userId,
            MainPassword = mainPassword,
            ExtraData = new RemoveRfidPinData
            {
                OldValue = oldValue
            }
        };
    }
}