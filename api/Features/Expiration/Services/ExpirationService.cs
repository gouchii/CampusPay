using api.Features.Expiration.Configs;
using api.Shared.Expiration.Enums;
using api.Shared.Expiration.Interfaces;
using Microsoft.Extensions.Options;

namespace api.Features.Expiration.Services;

public class ExpirationService : IExpirationService
{
    private readonly Dictionary<ExpirationType, TimeSpan> _expirationRules;

    //todo create a separate expiration service for each feature that needs it

    public ExpirationService(IOptions<ExpirationConfigTyped> config)
    {
        var rawRules = config.Value.Rules;
        _expirationRules = new Dictionary<ExpirationType, TimeSpan>();

        foreach (var kvp in rawRules)
        {
            if (Enum.TryParse<ExpirationType>(kvp.Key, out var key))
            {
                if (TimeSpan.TryParse(kvp.Value, out var timeSpan))
                {
                    _expirationRules[key] = timeSpan;
                }
                else
                {
                    throw new ArgumentException($"Invalid TimeSpan format for key '{kvp.Key}': '{kvp.Value}'");
                }
            }
            else
            {
                throw new ArgumentException($"Invalid ExpirationType key: '{kvp.Key}'");
            }
        }

        if (_expirationRules.Count == 0)
        {
            throw new InvalidOperationException("No valid expiration rules were loaded from configuration.");
        }
    }

    public bool IsExpired(DateTime createdAt, ExpirationType expirationType)
    {
        if (_expirationRules.TryGetValue(expirationType, out TimeSpan expiration))
        {
            return DateTime.Now - createdAt > expiration;
        }

        throw new InvalidOperationException($"No expiration rule defined for {expirationType}");
    }

    public TimeSpan GetRemainingTime(DateTime createdAt, ExpirationType expirationType)
    {
        if (_expirationRules.TryGetValue(expirationType, out TimeSpan expiration))
        {
            return expiration - (DateTime.UtcNow - createdAt);
        }

        throw new InvalidOperationException($"No expiration rule defined for {expirationType}");
    }
}