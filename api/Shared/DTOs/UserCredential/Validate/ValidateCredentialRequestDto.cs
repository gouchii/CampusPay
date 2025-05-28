using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.UserCredential.Validate;

public class ValidateCredentialRequestDto
{
    [Required] public string Value { get; set; } = string.Empty;
}