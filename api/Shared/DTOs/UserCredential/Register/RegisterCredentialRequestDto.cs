using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.UserCredential.Register;

public class RegisterCredentialRequestDto
{
    [Required] public string Value { get; set; } = string.Empty;
}