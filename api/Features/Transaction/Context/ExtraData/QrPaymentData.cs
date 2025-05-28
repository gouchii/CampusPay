namespace api.Features.Transaction.Context.ExtraData;

public class QrPaymentData : BaseExtraData
{
    public string SenderId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}