using api.Shared.DTOs.TransactionDto;
using api.Shared.DTOs.UserDto;
using api.Shared.DTOs.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace api.Features.SignalR;

[Authorize]
public class UserHub : Hub
{
    public async Task SendMessageToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine($"User connected: {userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        Console.WriteLine($"User disconnected: {userId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendTransaction(string userId, TransactionDto transactionDto)
    {
        await Clients.User(userId).SendAsync("ReceiveTransaction", transactionDto);
    }

    public async Task SendUserData(string userId, UserDto userDto)
    {
        await Clients.User(userId).SendAsync("UserData", userDto);
    }

    public async Task SendWalletData(string userId, WalletDto walletDto)
    {
        await Clients.User(userId).SendAsync("ReceiveWalletUpdate", walletDto);
    }
}