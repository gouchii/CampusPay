using api.DTOs.Transaction;
using api.DTOs.Wallet;
using api.Models;

namespace api.Interfaces.Service;

public interface ITransactionService
{
    Task<QrCodeDataDto> GenerateQrCodeAsync(string userId, decimal amount);

    Task<TransactionDto> VerifyQrScan(string transactionRef);
    Task<TransactionResultDto> ProcessQrPaymentAsync(string senderId, string token, string transactionRef);
}