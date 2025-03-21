using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Account;

public class RefreshTokenRequestDto
{
    [Required]public string RefreshToken { get; set; } = string.Empty;
}