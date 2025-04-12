namespace api.Features.Transaction.Models;

public class TransactionRelationModel
{
    public int Id { get; init; }

    public int ParentTransactionId { get; init; }
    public TransactionModel? ParentTransaction { get; init; }

    public int ChildTransactionId { get; init; }
    public TransactionModel? ChildTransaction { get; init; }
}