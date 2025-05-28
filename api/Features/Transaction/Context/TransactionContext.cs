using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Models;
using api.Shared.Enums.Transaction;

namespace api.Features.Transaction.Context;

public class TransactionContext
{
    public TransactionType Type { get; init; }
    public PaymentMethod Method { get; init; }
    public string TransactionRef { get; init; } = string.Empty;
    public BaseExtraData? ExtraData { get; init; }
}