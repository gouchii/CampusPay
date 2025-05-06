using api.Features.UserCredential.Context.Update.Builders;
using api.Features.UserCredential.Context.Update.Interfaces;
using api.Shared.Auth.Enums;

namespace api.Features.UserCredential.Context.Update.Factories;

public class UpdateCredentialContextBuilderFactory : IUpdateCredentialContextBuilderFactory
{
    private readonly Dictionary<CredentialType, Func<IUpdateCredentialContextBuilder>> _builderMap;

    public UpdateCredentialContextBuilderFactory(IServiceProvider serviceProvider)
    {
        _builderMap = new Dictionary<CredentialType, Func<IUpdateCredentialContextBuilder>>
        {
            { CredentialType.RfidPin, serviceProvider.GetRequiredService<UpdateRfidPinContextBuilder> },
            { CredentialType.RfidTag, serviceProvider.GetRequiredService<UpdateRfidTagContextBuilder> }
        };
    }

    public IUpdateCredentialContextBuilder GetBuilder(CredentialType type)
    {
        if (_builderMap.TryGetValue(type, out var builder))
        {
            return builder();
        }

        throw new NotSupportedException($"Builder for ({type}) not implemented.");
    }
}