using api.DTOs.QR;
using api.DTOs.Transaction;
using api.Extensions;
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

    [HttpPost("generate-qr")]
    public async Task<IActionResult> GenerateQrCode([FromBody] QrGenerateRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transactionDto = await _transactionService.VerifyQrScan(qrData.TransactionRef);
            return Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("ProcessQrPayment")]
    public async Task<IActionResult> ProcessQrPayment([FromBody] QrPaymentRequestDto qrData)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null)
            {
                return BadRequest();
            }

            var transactionResultDto = await _transactionService.ProcessQrPaymentAsync(userId, qrData.Token, qrData.TransactionRef);
            return Ok(transactionResultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}