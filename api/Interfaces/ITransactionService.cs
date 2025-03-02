using api.DTOs.Wallet;

namespace api.Interfaces;

public interface ITransactionService
{
    Task<string> GenerateQrCodeAsync(int userId, decimal amount);
    Task<TransactionResultDto> ProcessQrPaymentAsync(int scannerId, string qrData);
}