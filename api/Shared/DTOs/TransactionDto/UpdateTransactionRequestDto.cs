using System.ComponentModel.DataAnnotations;
using api.Features.Transaction.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace api.Shared.DTOs.TransactionDto;

public class UpdateTransactionRequestDto
{
    public decimal? Amount { get; init; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TransactionType? Type { get; init; }

    [JsonConverter(typeof(StringEnumConverter))]
    public PaymentMethod? Method { get; init; }
}