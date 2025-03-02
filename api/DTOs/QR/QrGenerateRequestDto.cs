using System.ComponentModel.DataAnnotations;

namespace api.DTOs.QR;

public class QrGenerateRequestDto
{
    [Required] public decimal Amount { get; set; }
}