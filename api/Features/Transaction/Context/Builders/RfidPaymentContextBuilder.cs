using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Enums;
using api.Features.Transaction.Interfaces;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Context.Builders;

public class RfidPaymentContextBuilder : ITransactionContextBuilder<BasePaymentRequestDto>
{
    private readonly ITransactionRepository _transactionRepo;
    public RfidPaymentContextBuilder(ITransactionRepository transactionRepo) => _transactionRepo = transactionRepo;


    public TransactionContext Build(BasePaymentRequestDto request, string uselessId)
    {
        string token;
        string transactionRef;
        string rfidTag;
        string rfidPin;
        string senderId;

        if (request is RfidPaymentRequestDto requestData)
        {
            token = requestData.Token;
            transactionRef = requestData.TransactionRef;
            rfidTag = requestData.RfidTag;
            rfidPin = requestData.RfidPin;
            senderId = requestData.SenderId;
        }
        else
        {
            throw new InvalidOperationException($"Expected PaymentRequestDto of type {nameof(RfidPaymentRequestDto)} but received {request.GetType().Name}.");
        }

        return new TransactionContext
        {
            TransactionRef = transactionRef,
            Type = TransactionType.Payment,
            Method = PaymentMethod.Rfid,
            ExtraData = new RfidPaymentData()
            {
                SenderId = senderId,
                Token = token,
                RfidTag = rfidTag,
                RfidPin = rfidPin
            }
        };
    }
}