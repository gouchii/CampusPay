using api.Features.UserCredential.Context.Remove;
using api.Features.UserCredential.Context.Update;
using api.Features.UserCredential.Interfaces;
using api.Features.UserCredential.Models;
using api.Shared.Auth.Enums;
using api.Shared.DTOs.UserCredential;
using api.Shared.DTOs.UserCredential.Validate;
using api.Shared.UserCredential.Interfaces;

namespace api.Features.UserCredential.Services;

public class UserCredentialService : IUserCredentialService
{
    private readonly ICredentialRegistrationHandlerFactory _registrationHandler;
    private readonly ICredentialRemoverHandlerFactory _removerHandler;
    private readonly ICredentialUpdateHandlerFactory _updateHandler;
    private readonly ICredentialValidatorHandlerFactory _validatorHandler;
    private readonly ICredentialVerificationHandlerFactory _verificationHandler;
    private readonly ICredentialFinderHandler _credentialFinder;

    public UserCredentialService(ICredentialRegistrationHandlerFactory registrationHandler, ICredentialRemoverHandlerFactory removerHandler, ICredentialUpdateHandlerFactory updateHandler,
        ICredentialValidatorHandlerFactory validatorHandler, ICredentialVerificationHandlerFactory verificationHandler, ICredentialFinderHandler credentialFinder)
    {
        _registrationHandler = registrationHandler;
        _removerHandler = removerHandler;
        _updateHandler = updateHandler;
        _validatorHandler = validatorHandler;
        _verificationHandler = verificationHandler;
        _credentialFinder = credentialFinder;
    }


    public async Task<bool> VerifyCredentialAsync(string userId, string value, CredentialType type)
    {
        var handler = _verificationHandler.GetHandler(type);
        return await handler.VerifyAsync(userId, value);
    }

    public async Task RegisterCredentialAsync(string userId, string value, CredentialType type)
    {
        var handler = _registrationHandler.GetHandler(type);
        await handler.RegisterAsync(userId, value);
    }

    public async Task RemoveCredentialAsync(RemoveCredentialContext context, CredentialType type)
    {
        var handler = _removerHandler.GetHandler(type);
        await handler.RemoveAsync(context);
    }

    public async Task UpdateCredentialAsync(UpdateCredentialContext context, CredentialType type)
    {
        var handler = _updateHandler.GetHandler(type);
        await handler.UpdateAsync(context);
    }

    public async Task<ValidateCredentialResultDto> ValidateCredentialAsync(string value, CredentialType type)
    {
        var handler = _validatorHandler.GetHandler(type);
        return await handler.ValidateAsync(value);
    }

    public async Task<UserCredentialModel?> FindCredentialAsync(string userId, CredentialType type)
    {
        return await _credentialFinder.GetByUserIdAsync(userId, type);
    }
}