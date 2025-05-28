using api.Features.UserCredential.Handlers.Validate;
using api.Features.UserCredential.Interfaces;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Factories;

public class CredentialValidatorHandlerFactory : ICredentialValidatorHandlerFactory
{
    private readonly Dictionary<CredentialType, Func<ICredentialValidatorHandler>> _handlerMap;

    public CredentialValidatorHandlerFactory(IServiceProvider serviceProvider)
    {
        _handlerMap = new Dictionary<CredentialType, Func<ICredentialValidatorHandler>>
        {
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<RfidTagValidatorHandler> }
        };
    }

    public ICredentialValidatorHandler GetHandler(CredentialType type)
    {
        if (_handlerMap.TryGetValue(type, out var handler))
        {
            return handler();
        }

        throw new NotSupportedException($"Handler for ({type}) not implemented.");
    }
}