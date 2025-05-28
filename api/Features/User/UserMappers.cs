using api.Shared.DTOs.UserDto;

namespace api.Features.User;

public static class UserMappers
{
    public static UserDto ToUserDto(this UserModel userModelModel, string role)
    {
        return new UserDto
        {
            FullName = userModelModel.FullName,
            UserName = userModelModel.UserName ?? string.Empty,
            Email = userModelModel.Email ?? string.Empty,
            Role = role,
            CreatedAt = userModelModel.CreatedAt
            // Wallets = userModel.Wallets.Select(w => w.ToWalletDto()).ToList(),
            // CreatedAt = userModel.CreatedAt,
            // SentTransactions = userModel.SentTransactions?
            //     .Select(t => t.ToTransactionDto())
            //     .ToList() ?? [],
            // ReceivedTransactions = userModel.ReceivedTransactions?
            //     .Select(t => t.ToTransactionDto())
            //     .ToList() ?? []
        };
    }


}