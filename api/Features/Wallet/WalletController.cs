using api.Features.Transaction.Interfaces;
using api.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Features.Wallet;
[Authorize]
[Route("api/wallet")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletRepository _walletRepo;

    public WalletController(IWalletRepository walletRepo, ITransactionService transactionService)
    {
        _walletRepo = walletRepo;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var wallets = await _walletRepo.GetAllAsync();
        return Ok(wallets.Select(w => w.ToWalletDto()));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAllWalletByUserId/{userId}")]
    public async Task<IActionResult> GetAllByUserId([FromRoute] string userId)
    {
        var wallets = await _walletRepo.GetAllByUserIdAsync(userId);

        return Ok(wallets.Select(w => w.ToWalletDto()));
    }


    [HttpGet("GetWallet")]
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