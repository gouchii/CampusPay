using api.Features.UserCredential.Handlers.Verify;
using api.Features.UserCredential.Interfaces;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Factories;

public class CredentialVerificationHandlerFactory : ICredentialVerificationHandlerFactory
{
    private readonly Dictionary<CredentialType, Func<ICredentialVerificationHandler>> _handlerMap;

    public CredentialVerificationHandlerFactory(IServiceProvider serviceProvider)
    {
        _handlerMap = new Dictionary<CredentialType, Func<ICredentialVerificationHandler>>
        {
            { CredentialType.RfidPin, serviceProvider.GetRequiredService<RfidPinVerificationHandler> },
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<RfidTagVerificationHandler> }
        };
    }

    public ICredentialVerificationHandler GetHandler(CredentialType type)
    {
        if (_handlerMap.TryGetValue(type, out var handler))
        {
            return handler();
        }

        throw new NotSupportedException($"Handler for ({type}) not implemented.");
    }
}