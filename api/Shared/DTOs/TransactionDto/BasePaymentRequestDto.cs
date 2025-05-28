using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.TransactionDto;

public abstract class BasePaymentRequestDto
{
    [Required] public string Token { get; set; } = string.Empty;
    [Required] public string TransactionRef { get; set; } = string.Empty;
}