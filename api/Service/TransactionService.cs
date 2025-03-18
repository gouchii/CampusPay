using System.Security.Cryptography;
using api.DTOs.Transaction;
using api.DTOs.Wallet;
using api.Enums;
using api.Interfaces;
using api.Mappers;
using api.Models;

namespace api.Service;

public class TransactionService : ITransactionService
{
    private readonly IUserRepository _userRepo;
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(IWalletRepository walletRepo, IUserRepository userRepo, ITransactionRepository transactionRepository)
    {
        _userRepo = userRepo;
        _walletRepo = walletRepo;
        _transactionRepository = transactionRepository;
    }

    public async Task<QrCodeDataDto> GenerateQrCodeAsync(string userId, decimal amount)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found.");

        //redundant??
        // if (amount <= 0) throw new Exception("Amount must be greater than zero.");

        var transactionRef = GenerateTransactionRef();
        var transactionModel = new Transaction()
        {
            ReceiverId = userId,
            Type = TransactionType.PaymentQr,
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

        if (transactionModel.Status != TransactionStatus.Pending) throw new Exception("Transaction Status is not Pending");

        var verificationToken = GenerateToken();

        transactionModel.VerificationToken = verificationToken;
        transactionModel.TokenGeneratedAt = DateTime.UtcNow;

        await _transactionRepository.UpdateTokenAsync(transactionModel);
        await _transactionRepository.UpdateTokenAsync(transactionModel);
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

        if (transactionModel.TokenGeneratedAt != null && (DateTime.UtcNow - transactionModel.TokenGeneratedAt.Value).TotalMinutes > 5)
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
        if (senderId == transactionModel.SenderId) throw new Exception("Invalid Action");

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

        await _transactionRepository.UpdateSenderAsync(transactionModel);
        await _transactionRepository.UpdateStatusAsync(transactionModel);
        await _transactionRepository.UpdateTokenAsync(transactionModel);
        await _transactionRepository.UpdateTokenTimeAsync(transactionModel);
        return new TransactionResultDto
        {
            Message = "Transaction Successful",
            ScannerBalance = senderWallet.Balance
        };
    }


    private static string GenerateTransactionRef()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(6)}";
    }

    private static string GenerateToken(int size = 32)
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(tokenBytes);
    }
}