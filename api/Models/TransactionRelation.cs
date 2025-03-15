namespace api.Models;

public class TransactionRelation
{
    public int Id { get; init; }

    public int ParentTransactionId { get; init; }
    public Transaction? ParentTransaction { get; set; }

    public int ChildTransactionId { get; init; }
    public Transaction? ChildTransaction { get; set; }
}