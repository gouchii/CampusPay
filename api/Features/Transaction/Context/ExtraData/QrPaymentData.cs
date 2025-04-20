namespace api.Features.Transaction.Context.ExtraData;

public class QrPaymentData : PaymentExtraData
{
    public string SenderId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}