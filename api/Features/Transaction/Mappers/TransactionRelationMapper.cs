using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Mappers;

public static class TransactionRelationMapper
{

    public static TransactionRelationDto ToTransactionRelationDto(this TransactionRelationModel transactionRelationModelModel)
    {
        return new TransactionRelationDto()
        {
            ParentTransactionId = transactionRelationModelModel.ParentTransactionId,
            ParentTransaction = new TransactionDto()
            {
                SenderName = transactionRelationModelModel.ParentTransaction?.Sender?.UserName,
                ReceiverName = transactionRelationModelModel.ParentTransaction?.Receiver?.UserName,
                Type = transactionRelationModelModel.ParentTransaction.Type,
                Amount = transactionRelationModelModel.ParentTransaction.Amount,
                CreatedAt = transactionRelationModelModel.ParentTransaction.CreatedAt,
                Status = transactionRelationModelModel.ParentTransaction.Status,
                TransactionRef = transactionRelationModelModel.ParentTransaction.TransactionRef
            },
            ChildTransactionId = transactionRelationModelModel.ChildTransactionId,
            ChildTransaction = new TransactionDto()
            {
                SenderName = transactionRelationModelModel.ChildTransaction?.Sender?.UserName,
                ReceiverName = transactionRelationModelModel.ChildTransaction?.Receiver?.UserName,
                Type = transactionRelationModelModel.ChildTransaction.Type,
                Amount = transactionRelationModelModel.ChildTransaction.Amount,
                CreatedAt = transactionRelationModelModel.ChildTransaction.CreatedAt,
                Status = transactionRelationModelModel.ChildTransaction.Status,
                TransactionRef = transactionRelationModelModel.ChildTransaction.TransactionRef
            }
        };
    }
}

// public int ParentTransactionId { get; set; }
// public TransactionDto ParentTransaction { get; set; }
//
// public int ChildTransactionId { get; set; }
// public TransactionDto ChildTransaction { get; set; }