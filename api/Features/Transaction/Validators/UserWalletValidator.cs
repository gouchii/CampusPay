using api.Features.Transaction.Interfaces;
using api.Features.Wallet;
using api.Shared.Wallet.Interfaces;

namespace api.Features.Transaction.Validators;

public class UserWalletValidator : IUserWalletValidator
{
    public void Validate(string senderId, string receiverId, WalletModel senderWallet, decimal amount)
    {
        //check if the sender is not trying to send money to themselves
        if (senderId == receiverId) throw new Exception("You cant send money to yourself");

        //check if the sender's wallet has enough funds
        if (senderWallet.Balance < amount) throw new Exception("Insufficient Balance");
    }
}