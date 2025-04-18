using api.Features.Auth.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;

namespace api.tests.Features.Auth.UserCredentialServiceTests;

public class RemoveCredentialTests : UserCredentialServiceTestsBase
{
    private const string UserId = "user";
    private const string RawValue = "user123";
    private const string HashedValue = "hashed";
    private const string WrongRawValue = "wrong";

    [Fact]
    public async Task RemoveCredentialAsync_UserNotFound_ThrowsException()
    {
        const CredentialType type = CredentialType.RfidPin;
        // Arrange
        A.CallTo(() => _userManager.FindByIdAsync(UserId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await _userCredentialService.RemoveCredentialAsync(UserId, RawValue, type);

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
        const CredentialType type = CredentialType.RfidPin;
        var user = new UserModel { Id = UserId };
        var credentialModel = new UserCredentialModel { HashedValue = HashedValue };


        A.CallTo(() => _userManager.FindByIdAsync(UserId)).Returns(user);
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, CredentialType.RfidPin)).Returns(credentialModel);
        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, HashedValue, WrongRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await _userCredentialService.RemoveCredentialAsync(UserId, WrongRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credential");
    }

    [Fact]
    public async Task RemoveCredentialAsync_WithValidUserAndCredential_RemovesCredential()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidPin;
        var user = new UserModel { Id = UserId };
        var credentialModel = new UserCredentialModel
        {
            UserId = UserId,
            HashedValue = HashedValue,
            Type = type
        };

        A.CallTo(() => _userManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, HashedValue, RawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await _userCredentialService.RemoveCredentialAsync(UserId, RawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.RemoveAsync(A<UserCredentialModel>.That.Matches(c =>
            c == credentialModel
        ))).MustHaveHappenedOnceExactly();
    }
}