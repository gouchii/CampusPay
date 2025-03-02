using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api.Enums;

namespace api.DTOs.Transaction;

public class CreateTransactionRequestDto
{


    [Required]
    public int ReceiverId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionType Type { get; set; }

    [Required]
    public decimal Amount { get; set; }




}