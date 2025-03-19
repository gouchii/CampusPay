using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api.Models;

public class User : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    [MaxLength(50)]
    public string? RfidTag { get; set; }

    [MaxLength(50)]
    public string? PinHash { get; set; }

    public List<Wallet> Wallets { get; set; } = new();
    public List<Transaction> SentTransactions { get; set; } = new();
    public List<Transaction> ReceivedTransactions { get; set; } = new();
}