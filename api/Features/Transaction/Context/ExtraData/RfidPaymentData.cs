namespace api.Features.Transaction.Context.ExtraData;

public class RfidPaymentData : BaseExtraData
{
    public string SenderId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string RfidTag { get; init; } = string.Empty;
    public string RfidPin { get; init; } = string.Empty;
}