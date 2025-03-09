using api.DTOs.Transaction;
using api.Enums;
using api.Models;

namespace api.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> CreateAsync(Transaction transactionModel);
    Task<Transaction?> UpdateStatusAsync(Transaction transactionModel);
    Task<Transaction?> UpdateTokenAsync(Transaction transactionModel);
    Task<Transaction?> UpdateTokenTimeAsync(Transaction transactionModel);
    Task<Transaction?> UpdateSenderAsync(Transaction transactionModel);
    Task<Transaction?> GetByTransactionRefAsync(string transactionRef);
}