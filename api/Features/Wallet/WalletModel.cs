using System.ComponentModel.DataAnnotations;
using api.Features.User;
using api.Shared.Enums.Wallet;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Wallet;

public class WalletModel
{
    public int Id { get; init; }

    [MaxLength(450)] public string UserId { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
    [Precision(18, 4)] public decimal Balance { get; set; }

    public UserModel? User { get; init; }

    [Required] public WalletType Type { get; init; }
}