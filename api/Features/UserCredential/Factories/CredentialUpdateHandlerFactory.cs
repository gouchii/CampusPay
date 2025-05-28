using api.Features.UserCredential.Handlers.Update;
using api.Features.UserCredential.Interfaces;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Factories;

public class CredentialUpdateHandlerFactory : ICredentialUpdateHandlerFactory
{
    private readonly Dictionary<CredentialType, Func<ICredentialUpdateHandler>> _handlerMap;

    public CredentialUpdateHandlerFactory(IServiceProvider serviceProvider)
    {
        _handlerMap = new Dictionary<CredentialType, Func<ICredentialUpdateHandler>>
        {
            { CredentialType.RfidPin, serviceProvider.GetRequiredService<RfidPinUpdateHandler> },
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<RfidTagUpdateHandler> }
        };
    }

    public ICredentialUpdateHandler GetHandler(CredentialType type)
    {
        if (_handlerMap.TryGetValue(type, out var handler))
        {
            return handler();
        }

        throw new NotSupportedException($"Handler for ({type}) not implemented.");
    }
}