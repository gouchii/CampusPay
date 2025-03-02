using api.DTOs.Transaction;
using api.DTOs.User;
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
            Timestamp = transactionModel.Timestamp,
            Status = transactionModel.Status,
            TransactionRef = transactionModel.TransactionRef,
            ParentRelations = transactionModel.ParentRelations?
                .Select(tr => tr.ToTransactionRelationDto()).ToList() ?? new List<TransactionRelationDto>(),
            ChildRelations = transactionModel.ChildRelations?
                .Select(tr => tr.ToTransactionRelationDto()).ToList() ?? new List<TransactionRelationDto>()

        };
    }
}