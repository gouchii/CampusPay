using api.DTOs.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service.Authorization;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletRepository _walletRepo;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> userManager, IWalletRepository walletRepo, ITokenService tokenService, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _walletRepo = walletRepo;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    public async Task<NewUserDto> Register(string userName,string fullName, string email, string password)
    {
        var user = new User
        {
            UserName = userName,
            FullName = fullName,
            Email = email
        };

        if (await _userManager.FindByEmailAsync(email) != null)
        {
            throw new Exception("Email is already in use.");
        }

        if (await _userManager.FindByNameAsync(userName) != null)
        {
            throw new Exception("Username is already taken.");
        }

        var createdUser = await _userManager.CreateAsync(user, password);

        if (!createdUser.Succeeded)
        {
            throw new Exception($"Registration failed: {string.Join(", ",
                createdUser.Errors.Select(e => e.Description))}");
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            throw new Exception($"Role assignment failed: {string.Join(", ",
                roleResult.Errors.Select(e => e.Description))}");
        }


        var wallet = await _walletRepo.CreateWalletAsync(user.Id);
        if (wallet == null)
        {
            throw new Exception("Failed to create default wallet.");
        }

        return new NewUserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = await _tokenService.CreateTokenAsync(user)
        };
    }

    public async Task<NewUserDto?> LogIn(string userName, string password)
    {
        //todo I dont know which one is better, change this if FindByNameAsync does not work
        // var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpper());
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("Invalid login credentials.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            throw new Exception("Invalid login credentials.");
        }
        return new NewUserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = await _tokenService.CreateTokenAsync(user)
        };
    }
}