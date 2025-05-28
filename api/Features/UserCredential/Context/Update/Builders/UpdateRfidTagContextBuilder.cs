using api.Features.UserCredential.Context.Update.ExtraData;
using api.Features.UserCredential.Context.Update.Interfaces;
using api.Shared.DTOs.UserCredential;
using api.Shared.DTOs.UserCredential.Update;

namespace api.Features.UserCredential.Context.Update.Builders;

public class UpdateRfidTagContextBuilder : IUpdateCredentialContextBuilder
{
    public UpdateCredentialContext Build(BaseUpdateCredentialRequestDto request, string userId)
    {
        string mainPassword;
        string newValue;

        if (request is UpdateRfidTagRequestDto requestData)
        {
            mainPassword = requestData.MainPassword;
            newValue = requestData.NewValue;
        }
        else
        {
            throw new InvalidOperationException($"Expected UpdateCredentialRequestDto of type {nameof(UpdateRfidTagRequestDto)} but received {request.GetType().Name}.");
        }

        return new UpdateCredentialContext
        {
            UserId = userId,
            MainPassword = mainPassword,
            NewValue = newValue,
            ExtraData = new UpdateRfidTagData()
        };
    }
}