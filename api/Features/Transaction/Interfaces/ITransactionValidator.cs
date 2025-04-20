using api.Features.Transaction.Models;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionValidator
{
    void ValidateForVerification(TransactionModel transaction);
    void ValidateForProcess(TransactionModel transaction, string token);
}