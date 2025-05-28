using api.Features.Auth.Interfaces;
using api.Features.Auth.Services;
using api.Features.User;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;

namespace api.tests.Features.Auth.UserCredentialServiceTests;

public abstract class UserCredentialServiceTestsBase
{
    protected readonly UserManager<UserModel> UserManager;
    protected readonly IUserCredentialRepository CredentialRepo;
    protected readonly IPasswordHasher<UserModel> PasswordHasher;
    protected readonly UserCredentialService UserCredentialService;


    protected UserCredentialServiceTestsBase()
    {
        UserManager = A.Fake<UserManager<UserModel>>();
        CredentialRepo = A.Fake<IUserCredentialRepository>();
        PasswordHasher = A.Fake<IPasswordHasher<UserModel>>();
        UserCredentialService = new UserCredentialService(CredentialRepo, PasswordHasher, UserManager);
    }
}