using api.DTOs.QR;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/wallet")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionService _transactionService;

    public WalletController(IWalletRepository walletRepo, ITransactionService transactionService)
    {
        _transactionService = transactionService;
        _walletRepo = walletRepo;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var wallets = await _walletRepo.GetAllAsync();
        return Ok((wallets.Select(w => w.ToWalletDto())));
    }

    [HttpGet("GetAllByUserId/{userId}")]
    public async Task<IActionResult> GetAllByUserId([FromRoute] int userId)
    {
        var wallets = await _walletRepo.GetAllByUserIdAsync(userId);

        return Ok(wallets.Select(w => w.ToWalletDto()));
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] int userId)
    {
        var wallet = await _walletRepo.GetByUserIdAsync(userId);

        return Ok(wallet.ToWalletDto());
    }

    [HttpPost("{scannerId}/scan")]
    public async Task<IActionResult> ScanQrCode([FromRoute] int scannerId, [FromBody] QrScanRequestDto scanRequest)
    {
        try
        {
            var result = await _transactionService.ProcessQrPaymentAsync(scannerId, scanRequest.QrData);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{userId}/generate-qr")]
    public async Task<IActionResult> GenerateQrCode([FromRoute] int userId, [FromBody] QrGenerateRequestDto request)
    {
        try
        {
            var qrData = await _transactionService.GenerateQrCodeAsync(userId, request.Amount);
            return Ok(new { QrCodeData = qrData });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}