using System.ComponentModel.DataAnnotations;
using api.Features.User;

namespace api.Features.Auth.Models;

public class RefreshTokenModel
{
    public int Id { get; init; }
    [MaxLength(44)]
    public string Token { get; init; } = string.Empty;

    [MaxLength(450)] public string UserId { get; init; } = string.Empty;
    public UserModel? User { get; init; }
    [Required]public DateTime CreatedAt { get; init; }
}