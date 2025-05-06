using api.Features.Transaction.Helpers;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionQueryHandler
{
    Task<List<TransactionDto>> GetAllAsync(string userId, TransactionQueryObject queryObject);
}