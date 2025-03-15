using api.DTOs.QR;
using api.DTOs.Transaction;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/transaction")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("{userId}/generate-qr")]
    public async Task<IActionResult> GenerateQrCode([FromRoute] int userId, [FromBody] QrGenerateRequestDto request)
    {
        try
        {
            var qrData = await _transactionService.GenerateQrCodeAsync(userId, request.Amount);
            return Ok(qrData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("verifyQrScan")]
    public async Task<IActionResult> VerifyQrScan([FromBody] QrCodeDataDto qrData)
    {
        try
        {
            var transactionDto = await _transactionService.VerifyQrScan(qrData.TransactionRef);
            return Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("ProcessQrPayment/{userId:int}")]
    public async Task<IActionResult> ProcessQrPayment([FromRoute] int userId,[FromBody] QrPaymentRequestDto qrData)
    {
        try
        {
            var transactionResultDto = await _transactionService.ProcessQrPaymentAsync(userId, qrData.Token, qrData.TransactionRef);
            return Ok(transactionResultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
}