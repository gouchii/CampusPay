using System.ComponentModel.DataAnnotations;
using api.Features.Auth.Models;
using api.Features.Transaction.Models;
using api.Features.Wallet;
using Microsoft.AspNetCore.Identity;

namespace api.Features.User;

public class UserModel : IdentityUser
{
    [Required] [MaxLength(100)] public string FullName { get; set; } = string.Empty;

    [MaxLength(5)] public int SchoolIdNumber { get; set; }

    public DateTime CreatedAt { get; init; }

    public List<RefreshTokenModel> RefreshTokens { get; init; } = [];
    public List<WalletModel> Wallets { get; init; } = [];
    public List<TransactionModel> SentTransactions { get; init; } = [];
    public List<TransactionModel> ReceivedTransactions { get; init; } = [];
    public List<UserCredentialModel> UserCredentials { get; set; } = [];
}