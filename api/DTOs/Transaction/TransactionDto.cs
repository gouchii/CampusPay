using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;
using api.Enums;
using api.Models;

namespace api.DTOs.Transaction;

public class TransactionDto
{

    public int? SenderId { get; set; }

    [Required]
    public int ReceiverId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionType Type { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionStatus Status { get; set; }

    [Required]
    [StringLength(36)]
    public string TransactionRef { get; set; }

    public List<TransactionRelationDto> ParentRelations { get; set; } = new();
    public List<TransactionRelationDto> ChildRelations { get; set; } = new();
}