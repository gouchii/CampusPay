using api.Features.UserCredential.Context.Update.ExtraData;

namespace api.Features.UserCredential.Context.Update;

public class UpdateCredentialContext
{
    public string UserId { get; set; } = string.Empty;
    public string MainPassword { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public BaseUpdateCredentialExtraData? ExtraData { get; set; }
}