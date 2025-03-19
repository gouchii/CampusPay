using System.ComponentModel.DataAnnotations;
using api.Enums;
using Microsoft.EntityFrameworkCore;


namespace api.Models;

public class Wallet
{
    public int Id { get; init; }

    [MaxLength(450)]
    public string UserId { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
    [Precision(18, 4)]
    public decimal Balance { get; set; }

    public User? User { get; set; }

    [Required]
    public WalletType Type { get; set; }
}