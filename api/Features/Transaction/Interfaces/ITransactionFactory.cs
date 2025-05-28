using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionFactory
{
    Task<TransactionRefDto> CreateTransactionAsync(string userId);
    Task<TransactionModel?> GetByTransactionRefAsync(string transactionRef);
}