using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;

namespace api.Features.Transaction.Factories;

public class TransactionFactory : ITransactionFactory
{
    private readonly ITransactionRepository _transactionRepo;

    public TransactionFactory(ITransactionRepository transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public async Task<TransactionModel> CreateTransactionAsync(string userId, decimal amount,
        TransactionType transactionType, PaymentMethod paymentMethod)
    {
        var transactionRef = GenerateTransactionRef();
        var transactionModel = new TransactionModel
        {
            ReceiverId = userId,
            Type = transactionType,
            Method = paymentMethod,
            Amount = amount,
            Status = TransactionStatus.Pending,
            TransactionRef = transactionRef,
        };
        await _transactionRepo.CreateAsync(transactionModel);
        return transactionModel;
    }

    public async Task<TransactionModel?> GetByTransactionRefAsync(string transactionRef)
    {
        return await _transactionRepo.GetByTransactionRefAsync(transactionRef);
    }

    private static string GenerateTransactionRef()
    {
        return $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(8)}";
    }
}