using api.DTOs.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletRepository _walletRepo;
    private readonly ITokenService _tokenService;


    public UserService(UserManager<User> userManager, IWalletRepository walletRepo, ITokenService tokenService)
    {
        _userManager = userManager;
        _walletRepo = walletRepo;
        _tokenService = tokenService;
    }


}