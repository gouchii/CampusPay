using api.Features.Auth.Models;
using api.Features.User;
using api.Shared.Auth.Enums;
using FakeItEasy;
using FluentAssertions;

namespace api.tests.Features.Auth.UserCredentialServiceTests;

public class RegisterCredentialTests : UserCredentialServiceTestsBase
{
    private const string UserId = "user";
    private const string RawValue = "user123";
    private const string HashedValue = "hashed";

    [Fact]
    public async Task RegisterCredentialAsync_UserNotFound_ThrowsException()
    {
        // Arrange
        A.CallTo(() => UserManager.FindByIdAsync(UserId)).Returns(Task.FromResult<UserModel?>(null));
        const CredentialType type = CredentialType.RfidPin;

        // Act
        var act = async () => await UserCredentialService.RegisterCredentialAsync(UserId, RawValue, type);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
    }


    [Fact]
    public async Task RegisterCredentialAsync_DuplicateRfidTag_ThrowsException()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidTag;
        var user = new UserModel { Id = UserId };
        var credentialModel = new UserCredentialModel
        {
            HashedValue = HashedValue,
            Type = type,
            UserId = UserId
        };

        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .Returns(user);

        A.CallTo(() => PasswordHasher.HashPassword(user, RawValue))
            .Returns(HashedValue);

        A.CallTo(() => CredentialRepo.GetByValueAsync(HashedValue, type))
            .ReturnsNextFromSequence(
                Task.FromResult<UserCredentialModel?>(null), // 1st call – allow registration
                Task.FromResult<UserCredentialModel?>(credentialModel) // 2nd call – simulate duplicate
            );

        // Act
        await UserCredentialService.RegisterCredentialAsync(UserId, RawValue, type);
        var act = async () => await UserCredentialService.RegisterCredentialAsync(UserId, RawValue, type);

        //Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Invalid RfidTag Value");
    }

    [Fact]
    public async Task RegisterCredentialAsync_WithValidUser_AddsCredential()
    {
        // Arrange
        const CredentialType type = CredentialType.RfidPin;
        var user = new UserModel { Id = UserId };


        A.CallTo(() => UserManager.FindByIdAsync(UserId))
            .Returns(Task.FromResult<UserModel?>(user));

        A.CallTo(() => PasswordHasher.HashPassword(user, RawValue))
            .Returns(HashedValue);

        // Act
        await UserCredentialService.RegisterCredentialAsync(UserId, RawValue, type);

        // Assert
        A.CallTo(() => CredentialRepo.AddAsync(A<UserCredentialModel>.That.Matches(c =>
            c.UserId == UserId &&
            c.HashedValue == HashedValue &&
            c.Type == type
        ))).MustHaveHappenedOnceExactly();
    }
}