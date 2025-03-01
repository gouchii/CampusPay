using api.DTOs.Wallet;
using api.Models;

namespace api.Mappers;

public static class WalletMappers
{
    public static WalletDto ToWalletDto(this Wallet walletModel)
    {
        return new WalletDto
        {
            UserId = walletModel.UserId,
            Balance = walletModel.Balance
        };
    }
}