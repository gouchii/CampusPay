using api.Features.Transaction.Interfaces;
using api.Shared.Extensions;
using api.Shared.Interfaces.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Wallet;

[Route("api/wallet")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletRepository _walletRepo;

    public WalletController(IWalletRepository walletRepo)
    {
        _walletRepo = walletRepo;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var wallets = await _walletRepo.GetAllAsync();
        return Ok(wallets.Select(w => w.ToWalletDto()));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all/{userId}")]
    public async Task<IActionResult> GetAllByUserId([FromRoute] string userId)
    {
        var wallets = await _walletRepo.GetAllByUserIdAsync(userId);

        return Ok(wallets.Select(w => w.ToWalletDto()));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetByUserId()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return BadRequest();
        }

        var wallet = await _walletRepo.GetByUserIdAsync(userId);

        if (wallet == null)
        {
            return BadRequest();
        }

        return Ok(wallet.ToWalletDto());
    }
}