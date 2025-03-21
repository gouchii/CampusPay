using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;

    [MaxLength(450)] public string UserId { get; init; } = string.Empty;
    public User? User { get; set; }
    [Required]public DateTime CreatedAt { get; init; }
}