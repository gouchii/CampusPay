using api.Features.UserCredential.Handlers.Register;
using api.Features.UserCredential.Interfaces;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Factories;

public class CredentialRegistrationHandlerFactory : ICredentialRegistrationHandlerFactory
{
    private readonly Dictionary<CredentialType, Func<ICredentialRegistrationHandler>> _handlerMap;

    public CredentialRegistrationHandlerFactory(IServiceProvider serviceProvider)
    {
        _handlerMap = new Dictionary<CredentialType, Func<ICredentialRegistrationHandler>>
        {
            { CredentialType.RfidPin, serviceProvider.GetRequiredService<RfidPinRegistrationHandler> },
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<RfidTagRegistrationHandler> }
        };
    }

    public ICredentialRegistrationHandler GetHandler(CredentialType type)
    {
        if (_handlerMap.TryGetValue(type, out var handler))
        {
            return handler();
        }

        throw new NotSupportedException($"Handler for ({type}) not implemented.");
    }
}