using api.Features.Transaction.Enums;

namespace api.Features.Transaction.Interfaces;

public interface ITransactionHandlerFactory
{
    public ITransactionHandler GetHandler(TransactionType type, PaymentMethod method);
}