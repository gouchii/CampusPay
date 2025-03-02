namespace api.DTOs.Transaction;

public class TransactionRelationDto
{
    public int ParentTransactionId { get; set; }
    public TransactionDto ParentTransaction { get; set; }

    public int ChildTransactionId { get; set; }
    public TransactionDto ChildTransaction { get; set; }
}