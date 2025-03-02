using api.DTOs.Transaction;
using api.Models;

namespace api.Mappers;

public static class TransactionRelationMapper
{

    public static TransactionRelationDto ToTransactionRelationDto(this TransactionRelation transactionRelationModel)
    {
        return new TransactionRelationDto()
        {
            ParentTransactionId = transactionRelationModel.ParentTransactionId,
            ParentTransaction = new TransactionDto()
            {
                SenderId = transactionRelationModel.ParentTransaction.SenderId,
                ReceiverId = transactionRelationModel.ParentTransaction.ReceiverId,
                Type = transactionRelationModel.ParentTransaction.Type,
                Amount = transactionRelationModel.ParentTransaction.Amount,
                Timestamp = transactionRelationModel.ParentTransaction.Timestamp,
                Status = transactionRelationModel.ParentTransaction.Status,
                TransactionRef = transactionRelationModel.ParentTransaction.TransactionRef
            },
            ChildTransactionId = transactionRelationModel.ChildTransactionId,
            ChildTransaction = new TransactionDto()
            {
                SenderId = transactionRelationModel.ChildTransaction.SenderId,
                ReceiverId = transactionRelationModel.ChildTransaction.ReceiverId,
                Type = transactionRelationModel.ChildTransaction.Type,
                Amount = transactionRelationModel.ChildTransaction.Amount,
                Timestamp = transactionRelationModel.ChildTransaction.Timestamp,
                Status = transactionRelationModel.ChildTransaction.Status,
                TransactionRef = transactionRelationModel.ChildTransaction.TransactionRef
            }
        };
    }
}

// public int ParentTransactionId { get; set; }
// public TransactionDto ParentTransaction { get; set; }
//
// public int ChildTransactionId { get; set; }
// public TransactionDto ChildTransaction { get; set; }