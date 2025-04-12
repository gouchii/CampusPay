using System.ComponentModel.DataAnnotations;

namespace api.Shared.DTOs.QR;

public class QrScanRequestDto
{
    [Required] public string QrData { get; set; } = string.Empty;
}