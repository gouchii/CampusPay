using api.Features.Transaction.Models;
using api.Features.Wallet;

namespace api.Features.Transaction.Interfaces;

public interface IUserWalletValidator
{
    void Validate(string senderId, string receiverId, WalletModel senderWallet, decimal amount);
}