namespace api.Features.UserCredential.Context.Remove.ExtraData;

public class RemoveRfidPinData : BaseRemoveCredentialExtraData
{
    public string OldValue { get; init; } = string.Empty;
}