namespace api.DTOs.Wallet;

public class TransactionResultDto
{
    public string Message { get; set; } = string.Empty;
    public decimal ScannerBalance { get; set; }
}