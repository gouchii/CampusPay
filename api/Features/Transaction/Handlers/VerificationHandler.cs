using System.Security.Cryptography;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Mappers;
using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Handlers;

public class VerificationHandler : IVerificationHandler
{
    private readonly ITransactionRepository _transactionRepo;
    private readonly ITransactionValidator _transactionValidator;

    public VerificationHandler(ITransactionRepository transactionRepo,
        ITransactionValidator transactionValidator)
    {
        _transactionRepo = transactionRepo;
        _transactionValidator = transactionValidator;
    }

    public async Task<TransactionDto> VerifyAsync(string transactionRef)
    {
        var transactionModel = await _transactionRepo.GetByTransactionRefAsync(transactionRef);

        if (transactionModel == null)
        {
            throw new Exception("Transaction not found");
        }

        _transactionValidator.ValidateForVerification(transactionModel);

        var verificationToken = GenerateToken();

        transactionModel.VerificationToken = verificationToken;
        transactionModel.TokenGeneratedAt = DateTime.Now;

        await _transactionRepo.UpdateAsync(transactionModel, [
            nameof(TransactionModel.VerificationToken),
            nameof(TransactionModel.TokenGeneratedAt)
        ]);

        return transactionModel.ToTransactionDto();
    }

    private static string GenerateToken(int size = 32)
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(tokenBytes);
    }
}