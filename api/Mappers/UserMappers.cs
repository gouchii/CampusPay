using api.DTOs.User;
using api.Models;

namespace api.Mappers;

public static class UserMappers
{
    public static UserDto ToUserDto(this User userModel, string role)
    {
        return new UserDto
        {
            FullName = userModel.FullName,
            UserName = userModel.UserName ?? string.Empty,
            Email = userModel.Email ?? string.Empty,
            Role = role,
            Wallets = userModel.Wallets.Select(w => w.ToWalletDto()).ToList(),
            CreatedAt = userModel.CreatedAt,
            SentTransactions = userModel.SentTransactions?
                .Select(t => t.ToTransactionDto())
                .ToList() ?? [],
            ReceivedTransactions = userModel.ReceivedTransactions?
                .Select(t => t.ToTransactionDto())
                .ToList() ?? []
        };
    }


}