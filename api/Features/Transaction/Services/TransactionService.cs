using api.Features.Transaction.Context;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Helpers;
using api.Features.Transaction.Interfaces;
using api.Shared.DTOs.TransactionDto;


namespace api.Features.Transaction.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionFactory _transactionFactory;
    private readonly IVerificationHandler _verificationHandler;
    private readonly ITransactionHandlerFactory _transactionHandlerFactory;
    private readonly ITransactionUpdateHandler _updateHandler;
    private readonly ITransactionQueryHandler _queryHandler;

    public TransactionService(ITransactionFactory transactionFactory,
        IVerificationHandler verificationHandler,
        ITransactionHandlerFactory transactionHandlerFactory, ITransactionUpdateHandler updateHandler, ITransactionQueryHandler queryHandler)
    {
        _transactionFactory = transactionFactory;
        _verificationHandler = verificationHandler;
        _transactionHandlerFactory = transactionHandlerFactory;
        _updateHandler = updateHandler;
        _queryHandler = queryHandler;
    }


    public async Task<TransactionRefDto> GenerateTransactionAsync(string userId)
    {
        return await _transactionFactory.CreateTransactionAsync(userId);
    }

    public async Task<TransactionDto> VerifyTransactionAsync(string transactionRef)
    {
        return await _verificationHandler.VerifyAsync(transactionRef);
    }

    public async Task<TransactionResultDto> ProcessTransactionAsync(TransactionContext context)
    {
        var transactionHandler = _transactionHandlerFactory.GetHandler(context.Type, context.Method);

        return await transactionHandler.HandleAsync(context);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(string userId, string transactionRef, UpdateTransactionRequestDto updateDto)
    {
        return await _updateHandler.UpdateAsync(userId, transactionRef, updateDto);
    }

    public async Task<List<TransactionDto>> GetAllAsync(string userId, TransactionQueryObject queryObject)
    {
        return await _queryHandler.GetAllAsync(userId, queryObject);
    }
}