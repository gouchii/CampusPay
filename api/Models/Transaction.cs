using System.ComponentModel.DataAnnotations;
using api.Enums;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public class Transaction
{
    public int Id { get; set; }

    [MaxLength(450)] public string? SenderId { get; set; }

    public User? Sender { get; set; }

    [MaxLength(450)] [Required] public string ReceiverId { get; init; } = string.Empty;

    public User? Receiver { get; set; }

    [Required] public TransactionType Type { get; init; }

    [Required]
    [Range(0.01, double.MaxValue)]
    [Precision(18, 4)]
    public decimal Amount { get; init; }

    [Required] public DateTime CreatedAt { get; init; }

    [Required] public TransactionStatus Status { get; set; }

    [Required] [StringLength(50)] public string TransactionRef { get; init; } = String.Empty;

    [StringLength(64)] public string? VerificationToken { get; set; }
    public DateTime? TokenGeneratedAt { get; set; }

    //for future stuffs
    public List<TransactionRelation> ParentRelations { get; set; } = new();
    public List<TransactionRelation> ChildRelations { get; set; } = new();
}