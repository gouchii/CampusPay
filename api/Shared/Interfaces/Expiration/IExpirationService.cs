using api.Shared.Enums.Expiration;

namespace api.Shared.Interfaces.Expiration;

public interface IExpirationService
{
    bool IsExpired(DateTime createdAt, ExpirationType expirationType);
    public TimeSpan GetRemainingTime(DateTime createdAt, ExpirationType expirationType);
}