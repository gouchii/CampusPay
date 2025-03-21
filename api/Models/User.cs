using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(254)]
    public string EmailAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Wallet> Wallets { get; set; } = new();

    public List<Transaction> SentTransactions { get; set; } = new();

    public List<Transaction> ReceivedTransactions { get; set; } = new();
}