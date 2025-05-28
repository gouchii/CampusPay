using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.UserCredential.Remove;

public class RemoveRfidPinRequestDto : BaseRemoveCredentialRequestDto
{
    [Required] public string OldValue { get; set; } = string.Empty;
}