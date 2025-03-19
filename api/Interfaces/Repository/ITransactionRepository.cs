using api.Models;

namespace api.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> CreateAsync(Transaction transactionModel);
    Task<Transaction?> UpdateAsync(Transaction transactionModel, params string[] updatedProperties);
    Task<Transaction?> GetByTransactionRefAsync(string transactionRef);
}