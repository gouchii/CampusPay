using api.Features.Transaction.Helpers;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Mappers;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Handlers;

public class TransactionQueryHandler : ITransactionQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<List<TransactionDto>> GetAllAsync(string userId, TransactionQueryObject queryObject)
    {
        var transactions = await _transactionRepository.GetAllByUserIdAsync(userId, queryObject);
        return transactions.Select(t => t.ToTransactionDto()).ToList();
    }
}