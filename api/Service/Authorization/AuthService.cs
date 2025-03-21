using System.Security.Cryptography;
using System.Text;
using api.DTOs.Account;
using api.Enums;
using api.Interfaces.Repository;
using api.Interfaces.Service;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service.Authorization;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IWalletRepository _walletRepo;
    private readonly IJwtService _jwtService;
    private readonly SignInManager<User> _signInManager;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IExpirationService _expirationService;
    private readonly IConfiguration _config;

    public AuthService (UserManager<User> userManager, IWalletRepository walletRepo,
        IJwtService jwtService, SignInManager<User> signInManager, IRefreshTokenRepository refreshTokenRepo,
        IExpirationService expirationService, IConfiguration config)
    {
        _userManager = userManager;
        _walletRepo = walletRepo;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _refreshTokenRepo = refreshTokenRepo;
        _expirationService = expirationService;
        _config = config;
    }

    public async Task<NewUserDto> Register(string userName, string fullName, string email, string password)
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
            AccessToken = await _jwtService.CreateTokenAsync(user),
            RefreshToken = await CreateRefreshTokenAsync(user.Id)
        };
    }

    public async Task<NewUserDto?> LogIn(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("Invalid login credentials.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid login credentials.");
        }

        return new NewUserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = await _jwtService.CreateTokenAsync(user),
            RefreshToken = await CreateRefreshTokenAsync(user.Id)
        };
    }
    //I don't know what I'm doing anymore T_T

    public async Task<NewUserDto?> RefreshJwtToken(string oldRefreshToken, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User does not exist");
        }

        return new NewUserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            AccessToken = await _jwtService.CreateTokenAsync(user),
            RefreshToken = await RefreshTokenAsync(oldRefreshToken, userId)
        };
    }

    public async Task Logout (string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User does not exist.");
        }

        var refreshTokens = await _refreshTokenRepo.GetByUserIdAsync(userId);
        if (refreshTokens.Any())
        {
            await _refreshTokenRepo.RemoveRangeAsync(refreshTokens);
        }
        await _signInManager.SignOutAsync();
    }

    private async Task<string> CreateRefreshTokenAsync(string userId)
    {
        var oldTokens = await _refreshTokenRepo.GetByUserIdAsync(userId);

        if (oldTokens.Any())
        {
            await _refreshTokenRepo.RemoveRangeAsync(oldTokens);
        }
        var token = _jwtService.GenerateRefreshToken();
        var hashedToken = HashToken(token);

        var newTokenModel = new RefreshToken
        {
            Token = hashedToken,
            UserId = userId
        };

        await _refreshTokenRepo.AddAsync(newTokenModel);

        return token;
    }

    //todo delete revoked RefreshTokens later

    private async Task<string> RefreshTokenAsync(string oldRefreshToken, string userId)
    {
        var hashedOldRefreshToken = HashToken(oldRefreshToken);
        var oldRefreshTokenModel = await _refreshTokenRepo.GetByTokenAsync(hashedOldRefreshToken);

        if (oldRefreshTokenModel == null ||
            _expirationService.IsExpired(oldRefreshTokenModel.CreatedAt, ExpirationType.RefreshToken) || oldRefreshTokenModel.UserId != userId)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }
        await _refreshTokenRepo.DeleteAsync(oldRefreshTokenModel);

        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newHashedRefreshToken = HashToken(newRefreshToken);

        var newTokenModel = new RefreshToken
        {
            Token = newHashedRefreshToken,
            UserId = oldRefreshTokenModel.UserId
        };

        await _refreshTokenRepo.AddAsync(newTokenModel);

        return newRefreshToken;
    }

    private string HashToken(string token)
    {
        var signingKey = _config["JWT:SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("JWT signing key is missing from configuration.");
        }
        var hmacKey = Encoding.UTF8.GetBytes(signingKey);
        using var hmac = new HMACSHA256(hmacKey);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }
}