using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;
using api.Shared.Enums.Expiration;
using api.Shared.Enums.Transaction;
using api.Shared.Interfaces.Expiration;

namespace api.Features.Transaction.Validators;

public class TransactionValidator : ITransactionValidator
{
    private readonly IExpirationService _expirationService;

    public TransactionValidator(IExpirationService expirationService)
    {
        _expirationService = expirationService;
    }

    public void ValidateForVerification(TransactionModel transactionModel)
    {
        if (transactionModel.Amount <= 0)
        {
            throw new Exception($"Transaction amount invalid : {transactionModel.Amount}");
        }

        // if (transactionModel.Method == PaymentMethod.None)
        // {
        //     throw new Exception("Payment method is set to none");
        // }
        //
        // if (transactionModel.Type == TransactionType.None)
        // {
        //     throw new Exception("Transaction type is set to none");
        // }

        if (transactionModel.TokenGeneratedAt != null && _expirationService.IsExpired(transactionModel.CreatedAt, ExpirationType.Transaction))
        {
            throw new Exception("Transaction Expired");
        }

        if (transactionModel.Status != TransactionStatus.Pending)
        {
            throw new Exception("Transaction Status is not Pending");
        }
    }

    //todo think of a better name for this method
    public void ValidateForProcess(TransactionModel transactionModel, string token)
    {
        if (transactionModel == null) throw new Exception("Transaction not found");


        if (transactionModel.VerificationToken != token) throw new Exception("Invalid token");


        if (transactionModel.TokenGeneratedAt != null &&
            _expirationService.IsExpired(transactionModel.TokenGeneratedAt.Value,
                ExpirationType.TransactionToken))
        {
            throw new Exception("Verification expired. Please re-verify.");
        }

        if (transactionModel.Status != TransactionStatus.Pending) throw new Exception("Transaction Already Completed");
    }
}