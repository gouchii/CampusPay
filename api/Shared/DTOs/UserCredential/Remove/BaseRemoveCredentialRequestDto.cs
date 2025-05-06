using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.UserCredential.Remove;

public abstract class BaseRemoveCredentialRequestDto
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public string MainPassword { get; set; } = string.Empty;
}