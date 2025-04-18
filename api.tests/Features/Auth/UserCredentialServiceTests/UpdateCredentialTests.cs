using api.Features.Auth.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;

namespace api.tests.Features.Auth.UserCredentialServiceTests;

public class UpdateCredentialTests : UserCredentialServiceTestsBase
{
    const string UserId = "user";
    const string OldRawValue = "user123";
    const string NewRawValue = "new-user123";
    const string WrongOldRawValue = "wrong";
    const string OldHashedValue = "hashed123";
    const string NewHashedValue = "new-hashed123";

    [Fact]
    public async Task UpdateCredentialAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidPin;
        A.CallTo(() => _userManager.FindByIdAsync(UserId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task UpdateCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        var user = new UserModel { Id = UserId };
        const CredentialType type = CredentialType.RfidPin;

        A.CallTo(() => _userManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, CredentialType.RfidPin))
            .Returns(Task.FromResult<UserCredentialModel?>(null));

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage($"User does not have a {type} registered yet");
    }

    [Fact]
    public async Task UpdateCredentialAsync_InValidCredential_ThrowsException()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidTag;
        var user = new UserModel { Id = UserId };
        var credentialModel = new UserCredentialModel { HashedValue = "hashed" };


        A.CallTo(() => _userManager.FindByIdAsync(UserId)).Returns(user);
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, CredentialType.RfidPin)).Returns(credentialModel);
        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, OldHashedValue, WrongOldRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(UserId, WrongOldRawValue, NewRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid credential");
    }

    [Fact]
    public async Task UpdateCredentialAsync_DuplicateRfidTag_ThrowsException()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidTag;
        var user = new UserModel { Id = UserId };
        var credentialModel = new UserCredentialModel
        {
            HashedValue = OldHashedValue,
            Type = type,
            UserId = UserId
        };

        A.CallTo(() => _userManager.FindByIdAsync(UserId))
            .Returns(user);

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, OldHashedValue, OldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var act = async () => await _userCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);
        //Assert

        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid RfidTag Value");
    }

    [Fact]
    public async Task UpdateCredentialAsync_WithValidUserAndCredential_UpdatesCredential()
    {
        // Arrange;
        const CredentialType type = CredentialType.RfidPin;
        var user = new UserModel { Id = UserId };

        var credentialModel = new UserCredentialModel
        {
            UserId = UserId,
            HashedValue = OldHashedValue,
            Type = type
        };

        A.CallTo(() => _userManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));
        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, OldHashedValue, OldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await _userCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.UpdateAsync(credentialModel,
            nameof(UserCredentialModel.HashedValue)
        )).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateCredentialAsync_WithValidUserAndCredentialAndRfidTag_UpdatesCredential()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidTag;
        var user = new UserModel { Id = UserId };

        var credentialModel = new UserCredentialModel
        {
            UserId = UserId,
            HashedValue = OldHashedValue,
            Type = type
        };

        A.CallTo(() => _userManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => _credentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => _passwordHasher.HashPassword(user, NewRawValue))
            .Returns(NewHashedValue);

        A.CallTo(() => _credentialRepo.GetByValueAsync(NewHashedValue, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(null));

        A.CallTo(() => _passwordHasher.VerifyHashedPassword(user, OldHashedValue, OldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await _userCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        A.CallTo(() => _credentialRepo.UpdateAsync(credentialModel,
            nameof(UserCredentialModel.HashedValue)
        )).MustHaveHappenedOnceExactly();
    }
}