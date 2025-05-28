using api.Features.Auth.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;

namespace api.tests.Features.Auth.UserCredentialServiceTests;

public class VerifyCredentialTests : UserCredentialServiceTestsBase
{
    private const string UserId = "user";
    private const string RawValue = "user123";
    private const string HashedValue = "hashed";
    private const string WrongRawValue = "wrong";


    [Fact]
    public async Task VerifyCredentialAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidPin;
        A.CallTo(() => UserManager.FindByIdAsync(UserId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await UserCredentialService.VerifyCredentialAsync(UserId, RawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task VerifyCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        var user = new UserModel { Id = UserId };
        const CredentialType type = CredentialType.RfidPin;

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(null));

        // Act
        var act = async () => await UserCredentialService.VerifyCredentialAsync(UserId, RawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage($"User does not have a {type} registered yet");
    }


    [Fact]
    public async Task VerifyCredentialAsync_ValidCredential_ReturnsTrue()
    {
        // Arrange
        const CredentialType credentialType = CredentialType.RfidPin;

        var user = new UserModel { Id = UserId };
        var credentialModel = new UserCredentialModel { HashedValue = HashedValue };

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, credentialType))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => PasswordHasher.VerifyHashedPassword(user, HashedValue, RawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = await UserCredentialService.VerifyCredentialAsync(UserId, RawValue, credentialType);

        // Assert
        result.Should().BeTrue();
        A.CallTo(() => CredentialRepo.UpdateAsync(credentialModel, nameof(UserCredentialModel.LastUsedAt))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task VerifyCredentialAsync_InvalidCredential_ReturnsFalse()
    {
        // Arrange
        var user = new UserModel { Id = UserId };
        var credential = new UserCredentialModel { HashedValue = HashedValue };

        A.CallTo(() => UserManager.FindByIdAsync(UserId)).Returns(user);
        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, CredentialType.RfidPin)).Returns(credential);
        A.CallTo(() => PasswordHasher.VerifyHashedPassword(user, HashedValue, WrongRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await UserCredentialService.VerifyCredentialAsync(UserId, WrongRawValue, CredentialType.RfidPin);

        // Assert
        result.Should().BeFalse();
        A.CallTo(() => CredentialRepo.UpdateAsync(A<UserCredentialModel>._, A<string>._)).MustNotHaveHappened();
    }
}