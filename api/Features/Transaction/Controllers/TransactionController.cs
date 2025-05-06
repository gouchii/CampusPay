using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Helpers;
using api.Features.Transaction.Interfaces;
using api.Shared.DTOs.QR;
using api.Shared.DTOs.TransactionDto;
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
    private readonly ITransactionContextBuilderFactory _transactionContextBuilder;

    public TransactionController(ITransactionService transactionService, ITransactionContextBuilderFactory transactionContextBuilder)
    {
        _transactionService = transactionService;
        _transactionContextBuilder = transactionContextBuilder;
    }

    [HttpPost("")]
    public async Task<IActionResult> GenerateTransaction()
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

            var qrData = await _transactionService.GenerateTransactionAsync(userId);
            return Ok(qrData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("verify/{transactionRef}")]
    public async Task<IActionResult> VerifyQrScan([FromRoute] string transactionRef)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var transactionDto = await _transactionService.VerifyTransactionAsync(transactionRef);
            return Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{transactionRef}")]
    public async Task<IActionResult> UpdateTransaction([FromRoute] string transactionRef, [FromBody] UpdateTransactionRequestDto requestDto)
    {
        try
        {
            var userId = User.GetUserId();
            if (userId == null) return BadRequest();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var transactionDto = await _transactionService.UpdateTransactionAsync(userId, transactionRef, requestDto);
            return Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("payment/qr/process")]
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

            var builder = _transactionContextBuilder.GetBuilder<BasePaymentRequestDto>(TransactionType.Payment, PaymentMethod.Qr);
            var context = builder.Build(request, userId);
            var transactionResultDto = await _transactionService.ProcessTransactionAsync(context);
            return Ok(transactionResultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("payment/rfid/process")]
    public async Task<IActionResult> ProcessRfidPayment([FromBody] RfidPaymentRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return BadRequest();

            var builder = _transactionContextBuilder.GetBuilder<BasePaymentRequestDto>(TransactionType.Payment, PaymentMethod.Rfid);
            var context = builder.Build(request, userId);
            var transactionResultDto = await _transactionService.ProcessTransactionAsync(context);
            return Ok(transactionResultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TransactionQueryObject query)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId == null) return BadRequest();

            var transactions = await _transactionService.GetAllAsync(userId, query);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}