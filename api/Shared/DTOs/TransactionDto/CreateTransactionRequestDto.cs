using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api.Features.Transaction.Enums;
using Newtonsoft.Json.Converters;

namespace api.Shared.DTOs.TransactionDto;

public class CreateTransactionRequestDto
{
    [Required]
    [JsonConverter(typeof(StringEnumConverter))]
    public TransactionType Type { get; set; }

    [Required] public decimal Amount { get; set; }
}