using api.DTOs.Account;
using api.Interfaces;
using api.Interfaces.Repository;
using api.Interfaces.Service;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletRepository _walletRepo;
    private readonly IJwtService _jwtService;


    public UserService(UserManager<User> userManager, IWalletRepository walletRepo, IJwtService jwtService)
    {
        _userManager = userManager;
        _walletRepo = walletRepo;
        _jwtService = jwtService;
    }


}