using System.ComponentModel.DataAnnotations;
using api.Features.User;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Models;

public class UserCredentialModel
{
    public int Id { get; init; }
    [Required] public string UserId { get; init; } = string.Empty;

    [Required] public CredentialType Type { get; init; }

    [Required] public string HashedValue { get; set; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? LastUsedAt { get; set; }

    public UserModel? User { get; init; }
}