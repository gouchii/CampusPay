using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace api.Shared.DTOs.TransactionDto;

public class QrGenerateRequestDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    [Precision(18, 4)]
    public decimal Amount { get; set; }
}