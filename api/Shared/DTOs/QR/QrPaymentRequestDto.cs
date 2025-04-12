using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.QR;

public class QrPaymentRequestDto
{
    [Required] public string Token { get; set; } = string.Empty;
    [Required] public string TransactionRef { get; set; } = string.Empty;
}