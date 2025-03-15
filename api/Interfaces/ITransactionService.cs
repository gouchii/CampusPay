using api.DTOs.Transaction;
using api.DTOs.Wallet;
using api.Models;

namespace api.Interfaces;

public interface ITransactionService
{
    Task<QrCodeDataDto> GenerateQrCodeAsync(int userId, decimal amount);

    Task<TransactionDto> VerifyQrScan(string transactionRef);
    Task<TransactionResultDto> ProcessQrPaymentAsync(int senderId, string token, string transactionRef);
}