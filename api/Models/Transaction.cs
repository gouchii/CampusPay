using System.ComponentModel.DataAnnotations;
using api.Enums;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public class Transaction
{
    public int Id { get; set; }

    public int? SenderId { get; set; }
    public User? Sender { get; set; }

    [Required] public int ReceiverId { get; set; }
    public User? Receiver { get; set; }

    [Required] public TransactionType Type { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    [Precision(18, 4)]
    public decimal Amount { get; set; }

    [Required] public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required] public TransactionStatus Status { get; set; }

    [Required] [StringLength(50)] public string TransactionRef { get; set; } = String.Empty;

    [StringLength(64)]
    public string? VerificationToken { get; set; }
    public DateTime? TokenGeneratedAt { get; set; }

    //for future stuffs
    public List<TransactionRelation> ParentRelations { get; set; } = new();
    public List<TransactionRelation> ChildRelations { get; set; } = new();
}