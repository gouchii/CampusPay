using api.DTOs.Transaction;
using api.Models;

namespace api.Interfaces;

public interface ITransactionRepository
{
    Task<bool> UpdateTransactionAsync(Transaction transactionModel);
    Task<Transaction?> GetByTransactionRef(string transactionRef);
}