using api.Enums;

namespace api.Interfaces.Service;

public interface IExpirationService
{
    bool IsExpired(DateTime createdAt, ExpirationType expirationType);
    public TimeSpan GetRemainingTime(DateTime createdAt, ExpirationType expirationType);
}