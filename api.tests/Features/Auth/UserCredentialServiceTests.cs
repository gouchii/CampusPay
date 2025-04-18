using FluentAssertions;
using FakeItEasy;
using api.Features.Auth.Services;
using api.Features.Auth.Interfaces;
using api.Features.Auth.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using Microsoft.AspNetCore.Identity;

namespace api.tests.Features.Auth;

public class UserCredentialServiceTests
{
    private readonly IUserCredentialRepository _credentialRepo;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly UserManager<UserModel> _userManager;
    private readonly UserCredentialService _userCredentialService;


    public UserCredentialServiceTests()
    {
        _credentialRepo = A.Fake<IUserCredentialRepository>();
        _passwordHasher = A.Fake<IPasswordHasher<UserModel>>();

        _userManager = A.Fake<UserManager<UserModel>>();
        _userCredentialService = new UserCredentialService(_credentialRepo, _passwordHasher, _userManager);
    }

    [Fact]
    public async Task VerifyCredentialAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        const string userId = "user";
        const string rawValue = "user123";
        const CredentialType type = CredentialType.RfidPin;
        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await _userCredentialService.VerifyCredentialAsync(userId, rawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task VerifyCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        const string userId = "user123";
        var user = new UserModel { Id = userId };
        const CredentialType type = CredentialType.RfidPin;

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, CredentialType.RfidPin))
            .Returns(Task.FromResult<UserCredentialModel?>(null));

        // Act
        var act = async () => await _userCredentialService.VerifyCredentialAsync("user123", "value", type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage($"User does not have a {type} registered yet");
    }


    [Fact]
    public async Task VerifyCredentialAsync_ValidCredential_ReturnsTrue()
    {
        // Arrange
        const string userId = "user123";
        const CredentialType credentialType = CredentialType.RfidPin;
        const string rawValue = "user123";
        const string hashedValue = "hashed";

        var user = new UserModel { Id = userId };
        var credential = new UserCredentialModel { HashedValue = hashedValue };


        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, credentialType))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credential));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, hashedValue, rawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = await _userCredentialService.VerifyCredentialAsync(userId, rawValue, credentialType);

        // Assert
        result.Should().BeTrue();
        A.CallTo(() => _credentialRepo.UpdateAsync(credential, nameof(UserCredentialModel.LastUsedAt))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task VerifyCredentialAsync_InvalidCredential_ReturnsFalse()
    {
        // Arrange
        const string userId = "user";
        var user = new UserModel { Id = userId };
        const string hashedValue = "hashed";
        const string wrongRawValue = "wrong";

        var credential = new UserCredentialModel { HashedValue = hashedValue };

        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, CredentialType.RfidPin)).Returns(credential);
        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, hashedValue, wrongRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _userCredentialService.VerifyCredentialAsync(userId, wrongRawValue, CredentialType.RfidPin);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterCredentialAsync_UserNotFound_ThrowsException()
    {
        const string userId = "user";
        const string rawValue = "user123";
        const CredentialType type = CredentialType.RfidPin;
        // Arrange
        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await _userCredentialService.RegisterCredentialAsync(userId, rawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }


    [Fact]
    public async Task RegisterCredentialAsync_DuplicateRfidTag_ThrowsException()
    {
        // Arrange
        const string userId = "user123";
        const string rfidTagValue = "rfid1234";
        const CredentialType type = CredentialType.RfidTag;
        const string hashedRfidValue = "hashed123";
        var user = new UserModel { Id = userId };
        var credentialModel = new UserCredentialModel
        {
            HashedValue = hashedRfidValue,
            Type = type,
            UserId = userId
        };

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .Returns(user);

        A.CallTo(() => _passwordHasher.HashPassword(user, rfidTagValue))
            .Returns(hashedRfidValue);

        A.CallTo(() => _credentialRepo.GetByValueAsync(hashedRfidValue, type))
            .ReturnsNextFromSequence(
                Task.FromResult<UserCredentialModel?>(null), // 1st call – allow registration
                Task.FromResult<UserCredentialModel?>(credentialModel) // 2nd call – simulate duplicate
            );

        // Act
        await _userCredentialService.RegisterCredentialAsync(userId, rfidTagValue, type);
        var act = async () => await _userCredentialService.RegisterCredentialAsync(userId, rfidTagValue, type);

        //Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid RfidTag Value");
    }

    [Fact]
    public async Task RegisterCredentialAsync_WithValidUser_AddsCredential()
    {
        // Arrange
        const string userId = "user-123";
        const string rawValue = "mySecret";
        const CredentialType type = CredentialType.RfidPin;
        const string hashedValue = "hashed-password";
        var user = new UserModel { Id = userId };


        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .Returns(Task.FromResult<UserModel?>(user));

        A.CallTo(() => _passwordHasher.HashPassword(user, rawValue))
            .Returns(hashedValue);

        // Act
        await _userCredentialService.RegisterCredentialAsync(userId, rawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.AddAsync(A<UserCredentialModel>.That.Matches(c =>
            c.UserId == userId &&
            c.HashedValue == hashedValue &&
            c.Type == type
        ))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RemoveCredentialAsync_UserNotFound_ThrowsException()
    {
        const string userId = "user";
        const string rawValue = "user123";
        const CredentialType type = CredentialType.RfidPin;
        // Arrange
        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await _userCredentialService.RemoveCredentialAsync(userId, rawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task RemoveCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        var user = new UserModel { Id = "user123" };
        const CredentialType type = CredentialType.RfidPin;

        A.CallTo(() => _userManager.FindByIdAsync("user123"))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));


        A.CallTo(() => _credentialRepo.GetByUserIdAsync("user123", type))
            .Returns(Task.FromResult<UserCredentialModel?>(null));

        // Act
        var act = async () => await _userCredentialService.RemoveCredentialAsync("user123", "value", CredentialType.RfidPin);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage($"User does not have a {type} registered yet");
    }

    [Fact]
    public async Task RemoveCredentialAsync_InValidCredential_ThrowsException()
    {
        // Arrange
        const string userId = "user123";
        const string wrongRawValue = "wrong";
        const string hashedValue = "user-123";
        const CredentialType type = CredentialType.RfidPin;
        var user = new UserModel { Id = userId };
        var credentialModel = new UserCredentialModel { HashedValue = hashedValue };


        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, CredentialType.RfidPin)).Returns(credentialModel);
        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, hashedValue, wrongRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await _userCredentialService.RemoveCredentialAsync(userId, wrongRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credential");
    }

    [Fact]
    public async Task RemoveCredentialAsync_WithValidUserAndCredential_RemovesCredential()
    {
        // Arrange
        const string userId = "user-123";
        const string rawValue = "mySecret";
        const CredentialType type = CredentialType.RfidPin;

        var user = new UserModel { Id = userId };
        const string hashedValue = "hashed-value";

        var credentialModel = new UserCredentialModel
        {
            UserId = userId,
            HashedValue = hashedValue,
            Type = type
        };

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, hashedValue, rawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await _userCredentialService.RemoveCredentialAsync(userId, rawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.RemoveAsync(A<UserCredentialModel>.That.Matches(c =>
            c == credentialModel
        ))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateCredentialAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        const string userId = "user";
        const string oldRawValue = "user123";
        const string newRawValue = "new-user123";
        const CredentialType type = CredentialType.RfidPin;
        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(userId, oldRawValue, newRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task UpdateCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        const string userId = "user";
        const string oldRawValue = "user123";
        const string newRawValue = "new-user123";
        var user = new UserModel { Id = userId };
        const CredentialType type = CredentialType.RfidPin;

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, CredentialType.RfidPin))
            .Returns(Task.FromResult<UserCredentialModel?>(null));

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(userId, oldRawValue, newRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage($"User does not have a {type} registered yet");
    }

    [Fact]
    public async Task UpdateCredentialAsync_InValidCredential_ThrowsException()
    {
        // Arrange
        const string userId = "user";
        const string newRawValue = "new-user123";
        const string wrongOldRawValue = "wrong";
        const string oldHashedValue = "hashed123";
        const CredentialType type = CredentialType.RfidTag;
        var user = new UserModel { Id = userId };
        var credentialModel = new UserCredentialModel { HashedValue = "hashed" };


        A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, CredentialType.RfidPin)).Returns(credentialModel);
        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, oldHashedValue, wrongOldRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(userId, wrongOldRawValue, newRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credential");
    }

    [Fact]
    public async Task UpdateCredentialAsync_DuplicateRfidTag_ThrowsException()
    {
        // Arrange
        const string userId = "user";
        const string oldRawValue = "user123";
        const string newRawValue = "new-user123";
        const CredentialType type = CredentialType.RfidTag;
        const string oldHashedValue = "hashed123";
        var user = new UserModel { Id = userId };
        var credentialModel = new UserCredentialModel
        {
            HashedValue = oldHashedValue,
            Type = type,
            UserId = userId
        };

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .Returns(user);

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, oldHashedValue, oldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(userId, oldRawValue, newRawValue, type);
        //Assert

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid RfidTag Value");
    }

    [Fact]
    public async Task UpdateCredentialAsync_WithValidUserAndCredential_UpdatesCredential()
    {
        // Arrange
        const string userId = "user";
        const string oldRawValue = "user123";
        const string newRawValue = "new-user123";
        const CredentialType type = CredentialType.RfidPin;
        const string oldHashedValue = "hashed123";

        var user = new UserModel { Id = userId };

        var credentialModel = new UserCredentialModel
        {
            UserId = userId,
            HashedValue = oldHashedValue,
            Type = type
        };

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, oldHashedValue, oldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await _userCredentialService.UpdateCredentialAsync(userId, oldRawValue, newRawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.UpdateAsync(credentialModel,
            nameof(UserCredentialModel.HashedValue)
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateCredentialAsync_WithValidUserAndCredentialAndRfidTag_UpdatesCredential()
    {
        // Arrange
        const string userId = "user";
        const string oldRawValue = "user123";
        const string newRawValue = "new-user123";
        const CredentialType type = CredentialType.RfidTag;
        const string oldHashedValue = "hashed123";
        const string newHashedValue = "new-hashed123";
        var user = new UserModel { Id = userId };

        var credentialModel = new UserCredentialModel
        {
            UserId = userId,
            HashedValue = oldHashedValue,
            Type = type
        };

        A.CallTo(() => _userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(userId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.HashPassword(user, newRawValue))
            .Returns(newHashedValue);

        A.CallTo(() => _credentialRepo.GetByValueAsync(newHashedValue, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(null));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, oldHashedValue, oldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await _userCredentialService.UpdateCredentialAsync(userId, oldRawValue, newRawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.UpdateAsync(credentialModel,
            nameof(UserCredentialModel.HashedValue)
        )).MustHaveHappenedOnceExactly();
    }
}