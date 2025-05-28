using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionUpdateHandler
{
    Task<TransactionDto> UpdateAsync(string userId, string transactionRef, UpdateTransactionRequestDto updateDto);
}