using api.Features.Transaction.Context;
using api.Features.Transaction.Helpers;
using api.Shared.DTOs.QR;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionService
{
    Task<TransactionRefDto> GenerateTransactionAsync(string userId);
    Task<TransactionDto> VerifyTransactionAsync(string transactionRef);
    Task<TransactionResultDto> ProcessTransactionAsync(TransactionContext context);
    Task<TransactionDto> UpdateTransactionAsync(string userId, string transactionRef, UpdateTransactionRequestDto updateDto);
    Task<List<TransactionDto>> GetAllAsync(string userId, TransactionQueryObject queryObject);
}