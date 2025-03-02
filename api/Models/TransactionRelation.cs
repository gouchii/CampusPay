namespace api.Models;

public class TransactionRelation
{
    public int Id { get; set; }

    public int ParentTransactionId { get; set; }
    public Transaction ParentTransaction { get; set; }

    public int ChildTransactionId { get; set; }
    public Transaction ChildTransaction { get; set; }
}