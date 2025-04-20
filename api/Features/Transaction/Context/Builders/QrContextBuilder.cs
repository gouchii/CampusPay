using api.Features.Transaction.Context.ExtraData;
using api.Features.Transaction.Context.Interfaces;
using api.Features.Transaction.Interfaces;
using api.Shared.DTOs.QR;

namespace api.Features.Transaction.Context.Builders;

public class QrContextBuilder : IQrContextBuilder
{
    private readonly ITransactionRepository _transactionRepo;

    public QrContextBuilder(ITransactionRepository transactionRepo) => _transactionRepo = transactionRepo;

    public async Task<TransactionContext> BuildAsync(QrPaymentRequestDto request, string senderId)
    {
        var model = await _transactionRepo.GetByTransactionRefAsync(request.TransactionRef);
        return new TransactionContext
        {
            Transaction = model,
            ExtraData = new QrPaymentData
            {
                SenderId = senderId,
                Token = request.Token
            }
        };
    }
}
//in controller
//var context = await _qrContextBuilder.BuildAsync(request);