using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.UserCredential.Update;

public class UpdateRfidPinRequestDto : BaseUpdateCredentialRequestDto
{
    [Required] public string OldValue { get; set; } = string.Empty;
}