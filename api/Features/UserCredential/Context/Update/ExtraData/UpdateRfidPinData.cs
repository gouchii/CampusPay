namespace api.Features.UserCredential.Context.Update.ExtraData;

public class UpdateRfidPinData : BaseUpdateCredentialExtraData
{
    public string OldValue { get; init; } = string.Empty;
}