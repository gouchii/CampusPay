using api.Shared.DTOs.QR;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Context.Interfaces;

public interface ITransactionContextBuilder<in TRequestDto>
{
    TransactionContext Build(TRequestDto request, string senderId);
}