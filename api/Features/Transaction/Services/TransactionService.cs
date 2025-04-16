using System.Security.Cryptography;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Mappers;
using api.Features.Transaction.Models;
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
    private readonly UserManager<User.UserModel> _userManager;
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IExpirationService _expirationService;

    public TransactionService(IWalletRepository walletRepo, UserManager<User.UserModel> userManager,
        ITransactionRepository transactionRepository, IExpirationService expirationService)
    {
        _userManager = userManager;
        _walletRepo = walletRepo;
        _transactionRepository = transactionRepository;
        _expirationService = expirationService;
    }

    public async Task<QrCodeDataDto> GenerateQrCodeAsync(string userId, decimal amount)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found.");

        var transactionRef = GenerateTransactionRef();
        var transactionModel = new TransactionModel()
        {
            ReceiverId = userId,
            Type = TransactionType.Payment,
            Amount = amount,
            Status = TransactionStatus.Pending,
            TransactionRef = transactionRef
        };
        await _transactionRepository.CreateAsync(transactionModel);
        var qrData = new QrCodeDataDto
        {
            TransactionRef = transactionRef
        };
        return qrData;
    }

    public async Task<TransactionDto> VerifyQrScan(string transactionRef)
    {
        var transactionModel = await _transactionRepository.GetByTransactionRefAsync(transactionRef);

        if (transactionModel == null)
        {
            throw new Exception("Transaction not found");
        }

        if (transactionModel.TokenGeneratedAt != null && _expirationService.IsExpired(transactionModel.CreatedAt, ExpirationType.Transaction))
        {
            throw new Exception("Transaction Expired");
        }

        if (transactionModel.Status != TransactionStatus.Pending) throw new Exception("Transaction Status is not Pending");

        var verificationToken = GenerateToken();

        transactionModel.VerificationToken = verificationToken;
        transactionModel.TokenGeneratedAt = DateTime.Now;


        await _transactionRepository.UpdateAsync(transactionModel, [
            nameof(TransactionModel.VerificationToken),
            nameof(TransactionModel.TokenGeneratedAt)
        ]);

        return transactionModel.ToTransactionDto();
    }


    public async Task<TransactionResultDto> ProcessQrPaymentAsync(string senderId, string token, string transactionRef)
    {
        //check if the transactionRef exists
        var transactionModel = await _transactionRepository.GetByTransactionRefAsync(transactionRef);

        if (transactionModel == null) throw new Exception("Transaction not found");

        //check if the token matches
        if (transactionModel.VerificationToken != token) throw new Exception("Invalid token");

        //check if token is expired

        if (transactionModel.TokenGeneratedAt != null && _expirationService.IsExpired(transactionModel.TokenGeneratedAt.Value, ExpirationType.TransactionToken))
        {
            throw new Exception("Verification expired. Please re-verify.");
        }

        //check if the transaction is already completed

        if (transactionModel.Status != TransactionStatus.Pending) throw new Exception("Transaction Already Completed");
        //check if the sender's wallet or the sender exists
        var senderWallet = await _walletRepo.GetByUserIdAsync(senderId);
        if (senderWallet == null) throw new Exception("Sender wallet not found");

        //check if the receiver's wallet or the receiver exists
        var receiverWallet = await _walletRepo.GetByUserIdAsync(transactionModel.ReceiverId);
        if (receiverWallet == null) throw new Exception("Receiver wallet not found");


        //check if the sender's wallet has enough funds
        if (senderWallet.Balance < transactionModel.Amount) throw new Exception("Insufficient Balance");

        //check if the sender is not trying to send money to themselves
        if (senderId == transactionModel.ReceiverId) throw new Exception("You cant send money to yourself");

        //check if the transaction is already processed
        if (transactionModel.Status == TransactionStatus.Completed) throw new Exception("Transaction Already Completed");

        //todo add some limits on how much you can send

        //process transaction and deduct funds
        senderWallet.Balance -= transactionModel.Amount;
        receiverWallet.Balance += transactionModel.Amount;

        //update wallet balances
        await _walletRepo.UpdateBalanceAsync(senderWallet);
        await _walletRepo.UpdateBalanceAsync(receiverWallet);

        //todo send confirmation by email or sms app notification

        // Proceed with payment
        transactionModel.Status = TransactionStatus.Completed;
        transactionModel.VerificationToken = null; // Invalidate token after successful payment
        transactionModel.TokenGeneratedAt = null; // Clear timestamp
        transactionModel.SenderId = senderId;
        transactionModel.Method = PaymentMethod.Qr;
        await _transactionRepository.UpdateAsync(transactionModel, new[]
        {
            nameof(TransactionModel.SenderId),
            nameof(TransactionModel.Status),
            nameof(TransactionModel.VerificationToken),
            nameof(TransactionModel.TokenGeneratedAt),
            nameof(TransactionModel.Method)
        });
        return new TransactionResultDto
        {
            Message = "Transaction Successful",
            ScannerBalance = senderWallet.Balance
        };
    }

    private static string GenerateTransactionRef()
    {
        // return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(6)}";
        return $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(8)}";
    }

    private static string GenerateToken(int size = 32)
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(tokenBytes);
    }
}