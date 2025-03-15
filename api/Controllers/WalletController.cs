using api.DTOs.QR;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/wallet")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletRepository _walletRepo;

    public WalletController(IWalletRepository walletRepo, ITransactionService transactionService)
    {
        _walletRepo = walletRepo;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var wallets = await _walletRepo.GetAllAsync();
        return Ok(wallets.Select(w => w.ToWalletDto()));
    }

    [HttpGet("GetAllByUserId/{userId}")]
    public async Task<IActionResult> GetAllByUserId([FromRoute] string userId)
    {
        var wallets = await _walletRepo.GetAllByUserIdAsync(userId);

        return Ok(wallets.Select(w => w.ToWalletDto()));
    }

    [HttpGet("GetByUserId/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] string userId)
    {
        var wallet = await _walletRepo.GetByUserIdAsync(userId);

        return Ok(wallet.ToWalletDto());
    }
}