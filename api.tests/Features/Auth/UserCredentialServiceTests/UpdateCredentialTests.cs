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
        A.CallTo(() => UserManager.FindByIdAsync(UserId)).Returns(Task.FromResult<UserModel?>(null));

        // Act
        var act = async () => await UserCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }

    [Fact]
    public async Task UpdateCredentialAsync_CredentialMissing_ThrowsException()
    {
        // Arrange
        var user = new UserModel { Id = UserId };
        const CredentialType type = CredentialType.RfidPin;

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, CredentialType.RfidPin))
            .Returns(Task.FromResult<UserCredentialModel?>(null));

        // Act
        var act = async () => await UserCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

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


        A.CallTo(() => UserManager.FindByIdAsync(UserId)).Returns(user);
        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, CredentialType.RfidPin)).Returns(credentialModel);
        A.CallTo(() => PasswordHasher.VerifyHashedPassword(user, OldHashedValue, WrongOldRawValue))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await UserCredentialService.UpdateCredentialAsync(UserId, WrongOldRawValue, NewRawValue, type);

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

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .Returns(user);

        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => PasswordHasher.VerifyHashedPassword(user, OldHashedValue, OldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var act = async () => await UserCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);
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

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));
        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => PasswordHasher.VerifyHashedPassword(user, OldHashedValue, OldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await UserCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        A.CallTo(() => CredentialRepo.UpdateAsync(credentialModel,
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

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .ReturnsLazily(() => Task.FromResult<UserModel?>(user));

        A.CallTo(() => CredentialRepo.GetByUserIdAsync(UserId, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(credentialModel));

        A.CallTo(() => PasswordHasher.HashPassword(user, NewRawValue))
            .Returns(NewHashedValue);

        A.CallTo(() => CredentialRepo.GetByValueAsync(NewHashedValue, type))
            .ReturnsLazily(() => Task.FromResult<UserCredentialModel?>(null));

        A.CallTo(() => PasswordHasher.VerifyHashedPassword(user, OldHashedValue, OldRawValue))
            .Returns(PasswordVerificationResult.Success);

        // Act
        await UserCredentialService.UpdateCredentialAsync(UserId, OldRawValue, NewRawValue, type);

        // Assert
        A.CallTo(() => CredentialRepo.UpdateAsync(credentialModel,
            nameof(UserCredentialModel.HashedValue)
        )).MustHaveHappenedOnceExactly();
    }
}