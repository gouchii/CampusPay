using api.DTOs.Wallet;

namespace api.DTOs.User;

public class UserDto
{
    public String Name { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public List<WalletDto> Wallets { get; set; }
}