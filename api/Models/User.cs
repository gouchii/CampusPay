using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Wallet> Wallets { get; set; } = new();

    public List<Transaction> SentTransactions { get; set; } = new();

    public List<Transaction> ReceivedTransactions { get; set; } = new();
}