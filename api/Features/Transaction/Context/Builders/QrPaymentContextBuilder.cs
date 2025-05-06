using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Context.Builders;

public class QrPaymentContextBuilder : ITransactionContextBuilder<BasePaymentRequestDto>
{
    private readonly ITransactionRepository _transactionRepo;

    public QrPaymentContextBuilder(ITransactionRepository transactionRepo) => _transactionRepo = transactionRepo;

    public TransactionContext Build(BasePaymentRequestDto request, string senderId)
    {
        string token;
        string transactionRef;
        if (request is QrPaymentRequestDto requestData)
        {
            token = requestData.Token;
            transactionRef = requestData.TransactionRef;
        }
        else
        {
            throw new InvalidOperationException($"Expected PaymentRequestDto of type {nameof(QrPaymentRequestDto)} but received {request.GetType().Name}.");
        }

        return new TransactionContext
        {
            TransactionRef = transactionRef,
            Type = TransactionType.Payment,
            Method = PaymentMethod.Qr,
            ExtraData = new QrPaymentData
            {
                SenderId = senderId,
                Token = token
            }
        };
    }
}