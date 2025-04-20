using api.Shared.DTOs.QR;

namespace api.Features.Transaction.Context.Interfaces;

public interface IQrContextBuilder
{
    Task<TransactionContext> BuildAsync(QrPaymentRequestDto request, string senderId);
}