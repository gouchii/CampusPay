using System.Text;
using System.Text.Json;
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
        _walletRepo = walletRepo;
        _userRepo = userRepo;
        _transactionRepository = transactionRepository;
    }

    public async Task<QrCodeDataDto> GenerateQrCodeAsync(int userId, decimal amount)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found.");

        if (amount <= 0) throw new Exception("Amount must be greater than zero.");

        string transactionRef = GenerateTransactionRef();
        var transactionModel = new Transaction()
        {
            ReceiverId = userId,
            Type = TransactionType.PaymentQr,
            Amount = amount,
            Status = TransactionStatus.Pending,
            TransactionRef = transactionRef
        };
        await _transactionRepository.UpdateTransactionAsync(transactionModel);
        var qrData = new QrCodeDataDto
        {
            TransactionRef = transactionRef
        };
        return qrData;
    }

    public async Task<TransactionDto> VerifyQrScan(string transactionRef)
    {
        var transactionModel = await _transactionRepository.GetByTransactionRef(transactionRef);
        if (transactionModel == null)
        {
            throw new Exception("Invalid QR code Data");
        }

        return transactionModel.ToTransactionDto();
    }


    // public async Task<TransactionResultDto> ProcessQrPaymentAsync(int scannerId, string qrData)
    // {
    //     QrCodeData? data;
    //     try
    //     {
    //         // Step 1: Decode from Base64
    //         var json = Encoding.UTF8.GetString(Convert.FromBase64String(qrData));
    //
    //         // Step 2: Deserialize JSON to object
    //         data = JsonSerializer.Deserialize<QrCodeData>(json);
    //     }
    //     catch
    //     {
    //         throw new Exception("Invalid QR data format.");
    //     }
    //
    //     if (data == null) throw new Exception("Failed to deserialize QR data");
    //
    //     var generatorId = data.GeneratorId;
    //     var amount = data.Amount;
    //
    //     var scannerWallet = await _walletRepo.GetByUserIdAsync(scannerId);
    //     var generatorWallet = await _walletRepo.GetByUserIdAsync(generatorId);
    //     if (scannerId == generatorId) throw new Exception("Invalid Operation");
    //     if (scannerWallet == null || generatorWallet == null) throw new Exception("One or both users do not have wallets");
    //
    //     if (scannerWallet.Balance < amount) throw new Exception("Insufficient Balance");
    //
    //     scannerWallet.Balance -= amount;
    //     generatorWallet.Balance += amount;
    //
    //     await _walletRepo.UpdateWalletAsync(scannerWallet);
    //     await _walletRepo.UpdateWalletAsync(generatorWallet);
    //
    //     return new TransactionResultDto
    //     {
    //         Message = "Transaction Successful",
    //         ScannerBalance = scannerWallet.Balance
    //     };
    // }


    public static string GenerateTransactionRef()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(6)}";
    }
}