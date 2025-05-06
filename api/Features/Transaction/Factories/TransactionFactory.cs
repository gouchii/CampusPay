using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Factories;

public class TransactionFactory : ITransactionFactory
{
    private readonly ITransactionRepository _transactionRepo;

    public TransactionFactory(ITransactionRepository transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public async Task<TransactionRefDto> CreateTransactionAsync(string userId)
    {
        var transactionRef = GenerateTransactionRef();
        var transactionModel = new TransactionModel
        {
            ReceiverId = userId,
            Status = TransactionStatus.Pending,
            TransactionRef = transactionRef,
        };
        await _transactionRepo.CreateAsync(transactionModel);
        return new TransactionRefDto
        {
            TransactionRef = transactionRef
        };
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