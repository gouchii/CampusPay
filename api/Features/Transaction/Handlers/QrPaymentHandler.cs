using System.Transactions;
using api.Features.Transaction.Context;
using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;
using api.Shared.Wallet.Interfaces;
using TransactionStatus = api.Features.Transaction.Enums.TransactionStatus;


namespace api.Features.Transaction.Handlers;

public class QrPaymentHandler : ITransactionHandler
{
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionValidator _transactionValidator;
    private readonly IUserWalletValidator _walletValidator;
    private readonly ITransactionRepository _transactionRepo;

    public QrPaymentHandler(IWalletRepository walletRepo,
        ITransactionRepository transactionRepo,
        ITransactionValidator transactionValidator,
        IUserWalletValidator walletValidator)
    {
        _walletRepo = walletRepo;
        _transactionRepo = transactionRepo;
        _transactionValidator = transactionValidator;
        _walletValidator = walletValidator;
    }

    public async Task<TransactionResultDto> HandleAsync(TransactionContext context)
    {
        string token;
        string senderId;

        if (context.ExtraData is QrPaymentData qrData)
        {
            senderId = qrData.SenderId;
            token = qrData.Token;
        }
        else
        {
            throw new InvalidOperationException($"Expected ExtraData of type {nameof(QrPaymentData)} but received {context.ExtraData?.GetType().Name ?? "null"}.");
        }

        var transactionModel = context.Transaction;

        if (transactionModel == null) throw new Exception("Transaction not found");

        _transactionValidator.ValidateForProcess(transactionModel, token);

        var senderWallet = await _walletRepo.GetByUserIdAsync(senderId);
        var receiverWallet = await _walletRepo.GetByUserIdAsync(transactionModel.ReceiverId);

        //check if the sender's wallet exists
        if (senderWallet == null) throw new Exception("Sender wallet not found");

        //check if the receiver's wallet exists

        if (receiverWallet == null) throw new Exception("Receiver wallet not found");

        _walletValidator.Validate(senderId, transactionModel.ReceiverId, senderWallet, transactionModel.Amount);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //process transaction and deduct funds
        senderWallet.Balance -= transactionModel.Amount;
        receiverWallet.Balance += transactionModel.Amount;

        //update wallet balances
        await _walletRepo.UpdateBalanceAsync(senderWallet);
        await _walletRepo.UpdateBalanceAsync(receiverWallet);

        transactionModel.Status = TransactionStatus.Completed;
        transactionModel.VerificationToken = null; // Invalidate token after successful payment
        transactionModel.TokenGeneratedAt = null; // Clear timestamp
        transactionModel.SenderId = senderId;
        transactionModel.Method = PaymentMethod.Qr;
        await _transactionRepo.UpdateAsync(transactionModel, new[]
        {
            nameof(TransactionModel.SenderId),
            nameof(TransactionModel.Status),
            nameof(TransactionModel.VerificationToken),
            nameof(TransactionModel.TokenGeneratedAt),
            nameof(TransactionModel.Method)
        });
        scope.Complete();

        return new TransactionResultDto
        {
            Message = "Transaction Successful",
            ScannerBalance = senderWallet.Balance
        };
    }
}