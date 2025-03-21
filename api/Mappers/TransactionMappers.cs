using api.DTOs.Transaction;
using api.Models;

namespace api.Mappers;

public static class TransactionMappers
{
    public static TransactionDto ToTransactionDto(this Transaction transactionModel)
    {
        return new TransactionDto()
        {
            SenderId = transactionModel.SenderId,
            ReceiverId = transactionModel.ReceiverId,
            Type = transactionModel.Type, //add the .ToString() if not working properly
            Amount = transactionModel.Amount,
            CreatedAt = transactionModel.CreatedAt,
            Status = transactionModel.Status,
            TransactionRef = transactionModel.TransactionRef,
            VerificationToken = transactionModel.VerificationToken,
            TokenGeneratedAt = transactionModel.TokenGeneratedAt,
            ParentRelations = transactionModel.ParentRelations?
                .Select(tr => tr.ToTransactionRelationDto()).ToList() ?? [],
            ChildRelations = transactionModel.ChildRelations?
                .Select(tr => tr.ToTransactionRelationDto()).ToList() ?? []

        };
    }
}