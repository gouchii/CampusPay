namespace api.Shared.DTOs.UserDto;

public class UserDto
{
    public string FullName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    // public List<WalletDto> Wallets { get; set; }
    //
    // public List<TransactionDto> SentTransactions { get; set; }
    //
    // public List<TransactionDto> ReceivedTransactions { get; set; }
}