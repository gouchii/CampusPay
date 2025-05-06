using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Mappers;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Handlers;

public class TransactionUpdateHandler : ITransactionUpdateHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public TransactionUpdateHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionDto> UpdateAsync(string userId, string transactionRef, UpdateTransactionRequestDto updateDto)
    {
        var transactionModel = await _transactionRepository.GetByTransactionRefAsync(transactionRef) ?? throw new Exception("Transaction Not found");

        if (transactionModel.ReceiverId != userId) throw new Exception("User does not have permission to edit this transaction");

        if (transactionModel.Status != TransactionStatus.Pending) throw new Exception("Editing this transaction is now forbidden");

        transactionModel.Amount = updateDto.Amount ?? transactionModel.Amount;
        transactionModel.Method = updateDto.Method ?? transactionModel.Method;
        transactionModel.Type = updateDto.Type ?? transactionModel.Type;
        transactionModel.VerificationToken = null;
        transactionModel.TokenGeneratedAt = null;
        var updated = await _transactionRepository.UpdateAsync(transactionModel, [
            nameof(transactionModel.Amount),
            nameof(transactionModel.Type),
            nameof(transactionModel.Method),
            nameof(transactionModel.VerificationToken),
            nameof(transactionModel.TokenGeneratedAt)
        ]) ?? throw new Exception("Something went wrong when updating the transaction");

        return updated.ToTransactionDto();
    }
}