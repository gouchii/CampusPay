using api.Shared.DTOs.QR;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionService
{
    Task<QrCodeDataDto> GenerateQrCodeAsync(string userId, decimal amount);
    Task<TransactionDto> VerifyQrScan(string transactionRef);
    Task<TransactionResultDto> ProcessQrPaymentAsync(string senderId, string token, string transactionRef);
}