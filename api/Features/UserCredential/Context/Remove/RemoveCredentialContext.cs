using api.Features.UserCredential.Context.Remove.ExtraData;

namespace api.Features.UserCredential.Context.Remove;

public class RemoveCredentialContext
{
    public string UserId { get; set; } = string.Empty;
    public string MainPassword { get; set; } = string.Empty;
    public BaseRemoveCredentialExtraData? ExtraData { get; set; }
}