using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api.Models;

public class User : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? RfidTag { get; set; } // Optional RFID, nullable until registered

    [MaxLength(50)]
    public string? PinHash { get; set; } // Hashed PIN, nullable until registered

    public List<Wallet> Wallets { get; set; } = new();
    public List<Transaction> SentTransactions { get; set; } = new();
    public List<Transaction> ReceivedTransactions { get; set; } = new();
}