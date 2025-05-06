using api.Features.Transaction.Enums;
using api.Features.Transaction.Handlers;
using api.Features.Transaction.Interfaces;

namespace api.Features.Transaction.Factories;

public class TransactionHandlerFactory : ITransactionHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITransactionHandler GetHandler(TransactionType type, PaymentMethod method)
    {
        return (type, method) switch
        {
            (TransactionType.Payment, PaymentMethod.Qr) => _serviceProvider.GetRequiredService<QrPaymentHandler>(),
            (TransactionType.Payment, PaymentMethod.Rfid) => _serviceProvider.GetRequiredService<RfidPaymentHandler>(),
            _ => throw new NotSupportedException("Handler not implemented")
        };
    }
}