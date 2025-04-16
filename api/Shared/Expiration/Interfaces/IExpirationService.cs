using api.Shared.Expiration.Enums;

namespace api.Shared.Expiration.Interfaces;

public interface IExpirationService
{
    bool IsExpired(DateTime createdAt, ExpirationType expirationType);
    public TimeSpan GetRemainingTime(DateTime createdAt, ExpirationType expirationType);
}