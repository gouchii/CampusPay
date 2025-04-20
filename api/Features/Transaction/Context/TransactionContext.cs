using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Models;

namespace api.Features.Transaction.Context;

public class TransactionContext
{
    public TransactionModel? Transaction { get; init; }
    public PaymentExtraData? ExtraData { get; init; }
}