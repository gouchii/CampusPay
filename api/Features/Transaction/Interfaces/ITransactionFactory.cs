using api.Features.Transaction.Enums;
using api.Features.Transaction.Models;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionFactory
{
    Task<TransactionModel> CreateTransactionAsync(string userId, decimal amount, TransactionType transactionType, PaymentMethod paymentMethod);
    Task<TransactionModel?> GetByTransactionRefAsync(string transactionRef);
}