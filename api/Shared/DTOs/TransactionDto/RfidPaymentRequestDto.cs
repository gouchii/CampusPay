using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.TransactionDto;

public class RfidPaymentRequestDto : BasePaymentRequestDto
{
    [Required] public string RfidTag { get; init; } = string.Empty;
    [Required] public string RfidPin { get; init; } = string.Empty;
    [Required] public string SenderId { get; init; } = string.Empty;
}