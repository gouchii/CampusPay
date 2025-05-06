using api.Features.Transaction.Context.Builders;
using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Enums;

namespace api.Features.Transaction.Context.Factories;

public class TransactionContextBuilderFactory : ITransactionContextBuilderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<(TransactionType, PaymentMethod), Type> _builderMap;

    public TransactionContextBuilderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _builderMap = new Dictionary<(TransactionType, PaymentMethod), Type>
        {
            { (TransactionType.Payment, PaymentMethod.Qr), typeof(QrPaymentContextBuilder) },
            { (TransactionType.Payment, PaymentMethod.Rfid), typeof(RfidPaymentContextBuilder) }
        };
    }

    public ITransactionContextBuilder<T> GetBuilder<T>(TransactionType type, PaymentMethod method)
    {
        if (_builderMap.TryGetValue((type, method), out var builderType))
        {
            var builder = _serviceProvider.GetRequiredService(builderType);
            if (builder is ITransactionContextBuilder<T> typedBuilder)
            {
                return typedBuilder;
            }

            throw new InvalidCastException($"Registered builder does not implement ITransactionContextBuilder<{typeof(T).Name}>");
        }

        throw new NotSupportedException($"No builder registered for ({type}, {method}).");
    }
}