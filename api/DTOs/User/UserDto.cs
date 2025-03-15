using api.DTOs.Transaction;
using api.DTOs.Wallet;

namespace api.DTOs.User;

public class UserDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<WalletDto> Wallets { get; set; }

    public List<TransactionDto> SentTransactions { get; set; }

    public List<TransactionDto> ReceivedTransactions { get; set; }

}