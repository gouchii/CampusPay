using System.Security.Cryptography;
using api.Features.Transaction.Context;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Mappers;
using api.Features.Transaction.Models;
using api.Features.User;
using api.Features.Wallet;
using api.Shared.DTOs.QR;
using api.Shared.DTOs.TransactionDto;
using api.Shared.Expiration.Enums;
using api.Shared.Expiration.Interfaces;
using api.Shared.Wallet.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace api.Features.Transaction.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionFactory _transactionFactory;
    private readonly IVerificationHandler _verificationHandler;
    private readonly ITransactionHandlerFactory _transactionHandlerFactory;

    public TransactionService(ITransactionFactory transactionFactory,
        IVerificationHandler verificationHandler,
        ITransactionHandlerFactory transactionHandlerFactory)
    {
        _transactionFactory = transactionFactory;
        _verificationHandler = verificationHandler;
        _transactionHandlerFactory = transactionHandlerFactory;
    }

    public async Task<QrCodeDataDto> GenerateTransactionAsync(string userId, decimal amount,
        TransactionType type, PaymentMethod method)
    {
        //I can make it so that transactionFactory spits out the transactionRef, but I am too lazy rn.
        var transactionModel = await _transactionFactory.CreateTransactionAsync(userId, amount, type, method);
        return new QrCodeDataDto
        {
            TransactionRef = transactionModel.TransactionRef
        };
    }

    public async Task<TransactionDto> VerifyTransactionAsync(string transactionRef)
    {
        return await _verificationHandler.VerifyAsync(transactionRef);
    }


    public async Task<TransactionResultDto> ProcessTransactionAsync(TransactionContext context)
    {
        if (context.Transaction == null)
        {
            throw new Exception("Transaction not found");
        }

        var transactionHandler = _transactionHandlerFactory.GetHandler(context.Transaction.Type, context.Transaction.Method);

        return await transactionHandler.HandleAsync(context);
    }
}