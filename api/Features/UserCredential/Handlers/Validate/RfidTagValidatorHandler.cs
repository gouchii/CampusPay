using api.Features.Auth.Interfaces;
using api.Features.User;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.DTOs.UserCredential.Validate;
using api.Shared.Enums.UserCredential;
using api.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Features.UserCredential.Handlers.Validate;

public class RfidTagValidatorHandler : ICredentialValidatorHandler
{
    private readonly IConfiguration _configuration;
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly UserManager<UserModel> _userManager;

    public RfidTagValidatorHandler(IConfiguration configuration, IUserCredentialRepository credentialRepository, UserManager<UserModel> userManager)
    {
        _configuration = configuration;
        _credentialRepository = credentialRepository;
        _userManager = userManager;
    }

    public async Task<ValidateCredentialResultDto> ValidateAsync(string value)
    {
        const CredentialType type = CredentialType.RfidTag;
        var signingKey = _configuration["UserCredentials:RfidTagSigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException("RfidTag signing key is missing from configuration.");
        }

        var hashedValue = TokenHasher.HashToken(value, signingKey);

        var credentialModel = await _credentialRepository.GetByValueAsync(hashedValue, type);
        if (credentialModel == null)
        {
            throw new Exception("Rfid Tag is invalid");
        }

        var userModel = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.Id == credentialModel.UserId);

        if (userModel == null)
        {
            throw new Exception("User not found");
        }

        if (!userModel.IsRfidPaymentEnabled)
        {
            throw new Exception("Rfid Payment is disabled");
        }

        // In case a user managed to remove their rfid pin without updating the isRfidPayment property in UserModel
        var rfidPinCheck = await _credentialRepository.GetByUserIdAsync(userModel.Id, CredentialType.RfidPin);
        if (rfidPinCheck == null)
        {
            throw new Exception($"User does not have a {CredentialType.RfidPin} registered yet");
        }

        credentialModel.LastUsedAt = DateTime.Now;

        await _credentialRepository.UpdateAsync(credentialModel, [
            nameof(UserCredentialModel.LastUsedAt)
        ]);

        return new ValidateCredentialResultDto
        {
            UserId = userModel.Id,
            UserName = userModel.UserName
        };
    }
}