using api.Features.Transaction.Helpers;
using api.Features.Transaction.Models;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionRepository
{
    Task<TransactionModel?> CreateAsync(TransactionModel transactionModelModel);
    Task<TransactionModel?> UpdateAsync(TransactionModel transactionModelModel, params string[] updatedProperties);
    Task<TransactionModel?> GetByTransactionRefAsync(string transactionRef);
    Task<List<TransactionModel>> GetAllByUserIdAsync(string userId, TransactionQueryObject query);
}