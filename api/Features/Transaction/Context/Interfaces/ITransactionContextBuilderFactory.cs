using api.Features.Transaction.Enums;

namespace api.Features.Transaction.Context.Interfaces;

public interface ITransactionContextBuilderFactory
{
    ITransactionContextBuilder<T> GetBuilder<T>(TransactionType type, PaymentMethod method);
}