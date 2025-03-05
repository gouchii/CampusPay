using api.DTOs.QR;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/transaction")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ITransactionRepository _transactionRepo;

    public TransactionController(ITransactionService transactionService, ITransactionRepository transactionRepoRepo)
    {
        _transactionService = transactionService;
        _transactionRepo = transactionRepoRepo;
    }


    // [HttpPost("{scannerId}/scan")]
    // public async Task<IActionResult> ScanQrCode([FromRoute] int scannerId, [FromBody] QrScanRequestDto scanRequest)
    // {
    //     try
    //     {
    //         var result = await _transactionService.ProcessQrPaymentAsync(scannerId, scanRequest.QrData);
    //         return Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex.Message);
    //     }
    // }

    [HttpPost("{userId}/generate-qr")]
    public async Task<IActionResult> GenerateQrCode([FromRoute] int userId, [FromBody] QrGenerateRequestDto request)
    {
        try
        {
            //todo should make this prettier
            var qrData = await _transactionService.GenerateQrCodeAsync(userId, request.Amount);
            return Ok(new { QrCodeData = qrData });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("verifyQrScan")]
    public async Task<IActionResult> VerifyQrScan([FromBody] string qrData)
    {
        try
        {
            var transactionDto = await _transactionService.VerifyQrScan(qrData);
            return Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    // [HttpPost("GetbyRef")]
    // public async Task<IActionResult> GetByRef([FromBody] QrCodeDataDto qrDataDto)
    // {
    //     var transactionModel = await _transactionRepo.GetByTransactionRef(qrDataDto.TransactionRef);
    //     if (transactionModel == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return Ok(transactionModel.ToTransactionDto());
    // }
}