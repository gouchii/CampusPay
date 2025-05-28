using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.UserCredential.Update;

public abstract class BaseUpdateCredentialRequestDto
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public string MainPassword { get; set; } = string.Empty;
    [Required] public string NewValue { get; set; } = string.Empty;
}