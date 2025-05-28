using api.Features.UserCredential.Context.Update.ExtraData;
using api.Features.UserCredential.Context.Update.Interfaces;
using api.Shared.DTOs.UserCredential;
using api.Shared.DTOs.UserCredential.Update;

namespace api.Features.UserCredential.Context.Update.Builders;

public class UpdateRfidPinContextBuilder : IUpdateCredentialContextBuilder
{
    public UpdateCredentialContext Build(BaseUpdateCredentialRequestDto request, string userId)
    {
        string mainPassword;
        string newValue;
        string oldValue;

        if (request is UpdateRfidPinRequestDto requestData)
        {
            mainPassword = requestData.MainPassword;
            newValue = requestData.NewValue;
            oldValue = requestData.OldValue;
        }
        else
        {
            throw new InvalidOperationException($"Expected UpdateRCredentialRequestDto of type {nameof(UpdateRfidPinRequestDto)} but received {request.GetType().Name}.");
        }

        return new UpdateCredentialContext
        {
            UserId = userId,
            MainPassword = mainPassword,
            NewValue = newValue,
            ExtraData = new UpdateRfidPinData
            {
                OldValue = oldValue
            }
        };
    }
}