using api.DTOs.QR;
using api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/transaction")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IWalletRepository _walletRepo;
    public TransactionController(ITransactionService transactionService, IWalletRepository walletRepo)
    {
        _transactionService = transactionService;
        _walletRepo = walletRepo;
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