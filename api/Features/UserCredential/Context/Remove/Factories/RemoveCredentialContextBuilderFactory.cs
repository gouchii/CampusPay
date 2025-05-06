using api.Features.UserCredential.Context.Remove.Builders;
using api.Features.UserCredential.Context.Remove.Interfaces;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Context.Remove.Factories;

public class RemoveCredentialContextBuilderFactory : IRemoveCredentialContextBuilderFactory
{
    private readonly Dictionary<CredentialType, Func<IRemoveCredentialContextBuilder>> _builderMap;

    public RemoveCredentialContextBuilderFactory(IServiceProvider serviceProvider)
    {
        _builderMap = new Dictionary<CredentialType, Func<IRemoveCredentialContextBuilder>>
        {
            { CredentialType.RfidPin, serviceProvider.GetRequiredService<RemoveRfidPinContextBuilder> },
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<RemoveRfidTagContextBuilder> }
        };
    }

    public IRemoveCredentialContextBuilder GetBuilder(CredentialType type)
    {
        if (_builderMap.TryGetValue(type, out var builder))
        {
            return builder();
        }

        throw new NotSupportedException($"Builder for ({type}) not implemented.");
    }
}