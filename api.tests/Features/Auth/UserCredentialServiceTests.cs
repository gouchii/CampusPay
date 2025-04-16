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
    [Fact]
    public async Task VerifyCredentialAsync_ValidCredential_ReturnsTrue()
    {
        // Arrange
        var userId = "user123";
        var credentialType = CredentialType.RfidPin;
        var rawPassword = "Secret123";
        var hashedPassword = "hashed";

        var user = new UserModel { Id = userId };
        var credential = new UserCredentialModel { HashedValue = hashedPassword };

        var credentialRepo = A.Fake<IUserCredentialRepository>();
        var passwordHasher = A.Fake<IPasswordHasher<UserModel>>();
        var userManager = A.Fake<UserManager<UserModel>>();

        A.CallTo(() => userManager.FindByIdAsync(userId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => credentialRepo.GetByTypeAsync(userId, credentialType))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credential));

        A.CallTo(() => passwordHasher.VerifyHashedPassword(user, hashedPassword, rawPassword))
            .Returns(PasswordVerificationResult.Success);

        var service = new UserCredentialService(credentialRepo, passwordHasher, userManager);

        // Act
        var result = await service.VerifyCredentialAsync(userId, rawPassword, credentialType);

        // Assert
        result.Should().BeTrue();
        A.CallTo(() => credentialRepo.UpdateAsync(credential, nameof(UserCredentialModel.LastUsedAt))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task VerifyCredentialAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        var userManager = A.Fake<UserManager<UserModel>>();
        A.CallTo(() => userManager.FindByIdAsync("missing")).Returns(Task.FromResult<UserModel?>(null));

        var service = new UserCredentialService(
            A.Fake<IUserCredentialRepository>(),
            A.Fake<IPasswordHasher<UserModel>>(),
            userManager
        );

        // Act
        var act = async () => await service.VerifyCredentialAsync("missing", "value", CredentialType.RfidPin);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task VerifyCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        var user = new UserModel { Id = "user123" };
        var userManager = A.Fake<UserManager<UserModel>>();
        // A.CallTo(() => userManager.FindByIdAsync("user123")).Returns(Task.FromResult(user));
        A.CallTo(() => userManager.FindByIdAsync("user123"))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        var credentialRepo = A.Fake<IUserCredentialRepository>();
        A.CallTo(() => credentialRepo.GetByTypeAsync("user123", CredentialType.RfidPin))
            .Returns(Task.FromResult<UserCredentialModel?>(null));

        var service = new UserCredentialService(credentialRepo, A.Fake<IPasswordHasher<UserModel>>(), userManager);

        // Act
        var act = async () => await service.VerifyCredentialAsync("user123", "value", CredentialType.RfidPin);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User does not have a RfidPin registered yet");
    }

    [Fact]
    public async Task VerifyCredentialAsync_WrongPassword_ReturnsFalse()
    {
        // Arrange
        var userId = "user123";
        var user = new UserModel { Id = userId };
        var credential = new UserCredentialModel { HashedValue = "hashed" };

        var userManager = A.Fake<UserManager<UserModel>>();
        var credentialRepo = A.Fake<IUserCredentialRepository>();
        var passwordHasher = A.Fake<IPasswordHasher<UserModel>>();

        A.CallTo(() => userManager.FindByIdAsync(userId)).Returns(user);
        A.CallTo(() => credentialRepo.GetByTypeAsync(userId, CredentialType.RfidPin)).Returns(credential);
        A.CallTo(() => passwordHasher.VerifyHashedPassword(user, "hashed", "wrongpass"))
            .Returns(PasswordVerificationResult.Failed);

        var service = new UserCredentialService(credentialRepo, passwordHasher, userManager);

        // Act
        var result = await service.VerifyCredentialAsync(userId, "wrongpass", CredentialType.RfidPin);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterCredentialAsync_WithValidUser_AddsCredential()
    {
        // Arrange
        var userId = "user-123";
        var password = "mySecret";
        var credentialType = CredentialType.RfidPin;

        var fakeUserManager = A.Fake<UserManager<UserModel>>(x =>
            x.WithArgumentsForConstructor(() => new UserManager<UserModel>(
                A.Fake<IUserStore<UserModel>>(),
                null, null, null, null, null, null, null, null)));

        var fakeHasher = A.Fake<IPasswordHasher<UserModel>>();
        var fakeRepo = A.Fake<IUserCredentialRepository>();

        var fakeUser = new UserModel { Id = userId };
        var hashedPassword = "hashed-password";

        A.CallTo(() => fakeUserManager.FindByIdAsync(userId))
            .Returns(Task.FromResult<UserModel?>(fakeUser));

        A.CallTo(() => fakeHasher.HashPassword(fakeUser, password))
            .Returns(hashedPassword);

        var service = new UserCredentialService(fakeRepo, fakeHasher, fakeUserManager);

        // Act
        await service.RegisterCredentialAsync(userId, password, credentialType);

        // Assert
        A.CallTo(() => fakeRepo.AddAsync(A<UserCredentialModel>.That.Matches(c =>
            c.UserId == userId &&
            c.HashedValue == hashedPassword &&
            c.Type == credentialType
        ))).MustHaveHappenedOnceExactly();
    }
}