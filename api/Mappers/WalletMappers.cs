using api.DTOs.Wallet;
using api.Models;

namespace api.Mappers;

public static class WalletMappers
{
    public static WalletDto ToWalletDto(this Wallet walletModel)
    {
        return new WalletDto
        {
            CreatedAt = walletModel.CreatedAt,
            Type = walletModel.Type,
            Balance = walletModel.Balance
        };
    }
}