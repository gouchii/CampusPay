using api.Shared.DTOs.Wallet;

namespace api.Features.Wallet;

public static class WalletMappers
{
    public static WalletDto ToWalletDto(this WalletModel walletModelModel)
    {
        return new WalletDto
        {
            CreatedAt = walletModelModel.CreatedAt,
            Type = walletModelModel.Type,
            Balance = walletModelModel.Balance
        };
    }
}