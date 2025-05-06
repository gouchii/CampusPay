using System.Transactions;
using api.Features.Transaction.Context;
using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using api.Shared.DTOs.TransactionDto;
using api.Shared.UserCredential.Interfaces;
using api.Shared.Wallet.Interfaces;
using Microsoft.AspNetCore.Identity;
using TransactionStatus = api.Features.Transaction.Enums.TransactionStatus;

namespace api.Features.Transaction.Handlers;

public class RfidPaymentHandler : ITransactionHandler
{
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionValidator _transactionValidator;
    private readonly IUserWalletValidator _walletValidator;
    private readonly ITransactionRepository _transactionRepo;
    private readonly IUserCredentialService _credentialService;
    private readonly UserManager<UserModel> _userManager;

    public RfidPaymentHandler(IWalletRepository walletRepo, ITransactionValidator transactionValidator,
        IUserWalletValidator walletValidator, ITransactionRepository transactionRepo,
        IUserCredentialService credentialService, UserManager<UserModel> userManager)
    {
        _walletRepo = walletRepo;
        _transactionValidator = transactionValidator;
        _walletValidator = walletValidator;
        _transactionRepo = transactionRepo;
        _credentialService = credentialService;
        _userManager = userManager;
    }

    public async Task<TransactionResultDto> HandleAsync(TransactionContext context)
    {
        string token;
        string senderId;
        string rfidTag;
        string rfidPin;

        if (context.ExtraData is RfidPaymentData rfidData)
        {
            token = rfidData.Token;
            rfidTag = rfidData.RfidTag;
            rfidPin = rfidData.RfidPin;
            senderId = rfidData.SenderId;
        }
        else
        {
            throw new InvalidOperationException($"Expected BasePaymentData of type {nameof(RfidPaymentData)} but received {context.ExtraData?.GetType().Name ?? "null"}.");
        }

        var transactionModel = await _transactionRepo.GetByTransactionRefAsync(context.TransactionRef) ?? throw new Exception("Transaction not found");

        var tagResult = await _credentialService.VerifyCredentialAsync(senderId, rfidTag, CredentialType.RfidTag);
        if (!tagResult) throw new Exception("Invalid Credentials");

        var pinResult = await _credentialService.VerifyCredentialAsync(senderId, rfidPin, CredentialType.RfidPin);
        if (!pinResult) throw new Exception("Invalid Credentials");

        var senderModel = await _userManager.FindByIdAsync(senderId) ?? throw new Exception("Sender not found");

        if (!senderModel.IsRfidPaymentEnabled) throw new Exception("Rfid payment is disabled");
        _transactionValidator.ValidateForProcess(transactionModel, token);

        var senderWallet = await _walletRepo.GetByUserIdAsync(senderId);
        var receiverWallet = await _walletRepo.GetByUserIdAsync(transactionModel.ReceiverId);

        if (senderWallet == null) throw new Exception("Sender wallet not found");

        if (receiverWallet == null) throw new Exception("Receiver wallet not found");

        _walletValidator.Validate(senderId, transactionModel.ReceiverId, senderWallet, transactionModel.Amount);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        senderWallet.Balance -= transactionModel.Amount;
        receiverWallet.Balance += transactionModel.Amount;

        await _walletRepo.UpdateBalanceAsync(senderWallet);
        await _walletRepo.UpdateBalanceAsync(receiverWallet);

        transactionModel.Status = TransactionStatus.Completed;
        transactionModel.VerificationToken = null;
        transactionModel.TokenGeneratedAt = null;
        transactionModel.SenderId = senderId;
        transactionModel.Method = PaymentMethod.Rfid;
        await _transactionRepo.UpdateAsync(transactionModel, [
            nameof(TransactionModel.SenderId),
            nameof(TransactionModel.Status),
            nameof(TransactionModel.VerificationToken),
            nameof(TransactionModel.TokenGeneratedAt),
            nameof(TransactionModel.Method)
        ]);
        scope.Complete();

        return new TransactionResultDto
        {
            Message = "Transaction Successful",
            ScannerBalance = senderWallet.Balance
        };
    }
}