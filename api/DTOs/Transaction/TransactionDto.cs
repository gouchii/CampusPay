using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;
using api.Enums;
using api.Models;
using Newtonsoft.Json.Converters;

namespace api.DTOs.Transaction;

public class TransactionDto
{

    public int? SenderId { get; set; }

    [Required]
    public int ReceiverId { get; set; }

    [Required]
    [JsonConverter(typeof(StringEnumConverter))]
    public TransactionType Type { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; }

    [Required]
    [JsonConverter(typeof(StringEnumConverter))]
    public TransactionStatus Status { get; set; }

    public string? VerificationToken { get; set; }
    public DateTime? TokenGeneratedAt { get; set; }

    [Required]
    [StringLength(36)]
    public string TransactionRef { get; set; } = string.Empty;

    public List<TransactionRelationDto> ParentRelations { get; set; } = new();
    public List<TransactionRelationDto> ChildRelations { get; set; } = new();
}