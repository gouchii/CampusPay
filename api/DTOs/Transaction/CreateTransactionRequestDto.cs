using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api.Enums;
using Newtonsoft.Json.Converters;

namespace api.DTOs.Transaction;

public class CreateTransactionRequestDto
{

    [Required]
    public string? ReceiverId { get; set; }

    [Required]
    [JsonConverter(typeof(StringEnumConverter))]
    public TransactionType Type { get; set; }

    [Required]
    public decimal Amount { get; set; }
}