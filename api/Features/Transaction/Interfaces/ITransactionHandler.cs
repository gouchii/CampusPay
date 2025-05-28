using api.Features.Transaction.Context;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionHandler
{
    Task<TransactionResultDto> HandleAsync(TransactionContext context);
}