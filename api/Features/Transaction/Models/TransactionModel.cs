using System.ComponentModel.DataAnnotations;
using api.Features.User;
using api.Shared.Enums.Transaction;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Transaction.Models;

public class TransactionModel
{
    public int Id { get; init; }

    [MaxLength(450)] public string? SenderId { get; set; }

    public UserModel? Sender { get; init; }

    [MaxLength(450)] [Required] public string ReceiverId { get; init; } = string.Empty;

    public UserModel? Receiver { get; init; }

    public TransactionType Type { get; set; } = TransactionType.None;

    public PaymentMethod Method { get; set; } = PaymentMethod.None;

    [Required]
    [Range(0.01, double.MaxValue)]
    [Precision(18, 4)]
    public decimal Amount { get; set; }

    [Required] public DateTime CreatedAt { get; init; }

    [Required] public TransactionStatus Status { get; set; }

    [Required] [StringLength(50)] public string TransactionRef { get; init; } = string.Empty;

    [StringLength(64)] public string? VerificationToken { get; set; }
    public DateTime? TokenGeneratedAt { get; set; }

    //for future stuffs
    public List<TransactionRelationModel> ParentRelations { get; set; } = new();
    public List<TransactionRelationModel> ChildRelations { get; set; } = new();
}