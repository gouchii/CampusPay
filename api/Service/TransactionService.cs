using System.Text;
using System.Text.Json;
using api.DTOs.Wallet;
using api.Interfaces;
using api.Models;

namespace api.Service;

public class TransactionService : ITransactionService
{
    private readonly IUserRepository _userRepo;
    private readonly IWalletRepository _walletRepo;

    public TransactionService(IWalletRepository walletRepo, IUserRepository userRepo)
    {
        _walletRepo = walletRepo;
        _userRepo = userRepo;
    }

    public async Task<string> GenerateQrCodeAsync(int userId, decimal amount)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found.");

        if (amount <= 0) throw new Exception("Amount must be greater than zero.");

        var qrData = new QrCodeData
        {
            GeneratorId = userId,
            Amount = amount
        };
        var json = JsonSerializer.Serialize(qrData);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }


    public async Task<TransactionResultDto> ProcessQrPaymentAsync(int scannerId, string qrData)
    {
        QrCodeData? data;
        try
        {
            // Step 1: Decode from Base64
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(qrData));

            // Step 2: Deserialize JSON to object
            data = JsonSerializer.Deserialize<QrCodeData>(json);
        }
        catch
        {
            throw new Exception("Invalid QR data format.");
        }

        if (data == null) throw new Exception("Failed to deserialize QR data");

        var generatorId = data.GeneratorId;
        var amount = data.Amount;

        var scannerWallet = await _walletRepo.GetByUserIdAsync(scannerId);
        var generatorWallet = await _walletRepo.GetByUserIdAsync(generatorId);
        if (scannerId == generatorId) throw new Exception("Invalid Operation");
        if (scannerWallet == null || generatorWallet == null) throw new Exception("One or both users do not have wallets");

        if (scannerWallet.Balance < amount) throw new Exception("Insufficient Balance");

        scannerWallet.Balance -= amount;
        generatorWallet.Balance += amount;

        await _walletRepo.UpdateWalletAsync(scannerWallet);
        await _walletRepo.UpdateWalletAsync(generatorWallet);

        return new TransactionResultDto
        {
            Message = "Transaction Successful",
            ScannerBalance = scannerWallet.Balance
        };
    }


    public static string GenerateTransactionRef()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(6)}";
    }
}