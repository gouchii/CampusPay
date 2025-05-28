using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.QR;

public class QrCodeDataDto
{
    [Required] public string TransactionRef { get; set; } = String.Empty;
}