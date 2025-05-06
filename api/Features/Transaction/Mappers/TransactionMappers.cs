using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Mappers;

public static class TransactionMappers
{
    public static TransactionDto ToTransactionDto(this TransactionModel transactionModelModel)
    {
        return new TransactionDto()
        {
            SenderName = transactionModelModel.Sender?.UserName,
            ReceiverName = transactionModelModel.Receiver?.UserName,
            Type = transactionModelModel.Type, //add the .ToString() if not working properly
            Method = transactionModelModel.Method,
            Amount = transactionModelModel.Amount,
            CreatedAt = transactionModelModel.CreatedAt,
            Status = transactionModelModel.Status,
            TransactionRef = transactionModelModel.TransactionRef,
            VerificationToken = transactionModelModel.VerificationToken,
            TokenGeneratedAt = transactionModelModel.TokenGeneratedAt,
            ParentRelations = transactionModelModel.ParentRelations?
                .Select(tr => tr.ToTransactionRelationDto()).ToList() ?? [],
            ChildRelations = transactionModelModel.ChildRelations?
                .Select(tr => tr.ToTransactionRelationDto()).ToList() ?? []
        };
    }
}