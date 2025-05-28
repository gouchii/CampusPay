namespace api.Shared.DTOs.TransactionDto;

public class TransactionResultDto
{
    public string Message { get; set; } = string.Empty;
    public decimal ScannerBalance { get; set; }
}