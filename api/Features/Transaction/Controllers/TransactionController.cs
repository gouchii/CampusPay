using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Shared.DTOs.QR;
using api.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Transaction.Controllers;

[Authorize]
[Route("api/transaction")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IQrContextBuilder _qrContextBuilder;

    public TransactionController(ITransactionService transactionService, IQrContextBuilder qrContextBuilder)
    {
        _transactionService = transactionService;
        _qrContextBuilder = qrContextBuilder;
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

            var qrData = await _transactionService.GenerateTransactionAsync(userId, request.Amount, TransactionType.Payment, PaymentMethod.Qr);
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

            var transactionDto = await _transactionService.VerifyTransactionAsync(qrData.TransactionRef);
            return Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("ProcessQrPayment")]
    public async Task<IActionResult> ProcessQrPayment([FromBody] QrPaymentRequestDto request)
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

            var context = await _qrContextBuilder.BuildAsync(request, userId);
            var transactionResultDto = await _transactionService.ProcessTransactionAsync(context);
            return Ok(transactionResultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}