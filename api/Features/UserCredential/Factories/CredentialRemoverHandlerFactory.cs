using api.Features.UserCredential.Handlers.Remove;
using api.Features.UserCredential.Interfaces;
using api.Shared.Enums.UserCredential;

namespace api.Features.UserCredential.Factories;

public class CredentialRemoverHandlerFactory : ICredentialRemoverHandlerFactory
{
    private readonly Dictionary<CredentialType, Func<ICredentialRemoverHandler>> _handlerMap;

    public CredentialRemoverHandlerFactory(IServiceProvider serviceProvider)
    {
        _handlerMap = new Dictionary<CredentialType, Func<ICredentialRemoverHandler>>
        {
            { CredentialType.RfidPin, serviceProvider.GetRequiredService<RfidPinRemoverHandler> },
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<RfidTagRemoverHandler> }
        };
    }

    public ICredentialRemoverHandler GetHandler(CredentialType type)
    {
        if (_handlerMap.TryGetValue(type, out var handler))
        {
            return handler();
        }

        throw new NotSupportedException($"Handler for ({type}) not implemented.");
    }
}