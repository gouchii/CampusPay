using api.Features.Transaction.Context;
using api.Features.Transaction.Enums;
using api.Shared.DTOs.QR;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionService
{
    Task<QrCodeDataDto> GenerateTransactionAsync(string userId, decimal amount, TransactionType type, PaymentMethod method);
    Task<TransactionDto> VerifyTransactionAsync(string transactionRef);
    Task<TransactionResultDto> ProcessTransactionAsync(TransactionContext context);
}