using api.Shared.Enum;

namespace api.Shared.Interface;

public interface IExpirationService
{
    bool IsExpired(DateTime createdAt, ExpirationType expirationType);
    public TimeSpan GetRemainingTime(DateTime createdAt, ExpirationType expirationType);
}