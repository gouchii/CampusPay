using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.Authentication;

public class RefreshTokenRequestDto
{
    [Required]public string RefreshToken { get; set; } = string.Empty;
}