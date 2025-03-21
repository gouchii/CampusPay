using api.Enums;
using api.Interfaces;
using api.Interfaces.Service;
using api.Models;

namespace api.Service;

public class ExpirationService : IExpirationService
{
    private readonly Dictionary<ExpirationType, TimeSpan> _expirationRules;

    public ExpirationService()
    {
        _expirationRules = new Dictionary<ExpirationType, TimeSpan>
        {
            {ExpirationType.Transaction, TimeSpan.FromHours(1)},
            {ExpirationType.TransactionToken, TimeSpan.FromMinutes(5)},
            {ExpirationType.RefreshToken, TimeSpan.FromDays(1)}
        };
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