using api.Shared.Enums.Transaction;

namespace api.Features.Transaction.Context.Interfaces;

public interface ITransactionContextBuilderFactory
{
    ITransactionContextBuilder<T> GetBuilder<T>(TransactionType type, PaymentMethod method);
}