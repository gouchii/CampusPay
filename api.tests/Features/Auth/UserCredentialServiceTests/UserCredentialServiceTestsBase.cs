using api.Features.Auth.Interfaces;
using api.Features.Auth.Services;
using api.Features.User;
using api.Shared.Auth.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;

namespace api.tests.Features.Auth.UserCredentialServiceTests;

public abstract class UserCredentialServiceTestsBase
{
    protected readonly UserManager<UserModel> _userManager;
    protected readonly IUserCredentialRepository _credentialRepo;
    protected readonly IPasswordHasher<UserModel> _passwordHasher;
    protected readonly UserCredentialService _userCredentialService;


    protected UserCredentialServiceTestsBase()
    {
        _userManager = A.Fake<UserManager<UserModel>>();
        _credentialRepo = A.Fake<IUserCredentialRepository>();
        _passwordHasher = A.Fake<IPasswordHasher<UserModel>>();
        _userCredentialService = new UserCredentialService(_credentialRepo, _passwordHasher, _userManager);
    }
}