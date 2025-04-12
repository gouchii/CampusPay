namespace api.Shared.DTOs.TransactionDto;

public class TransactionRelationDto
{
    public int ParentTransactionId { get; set; }
    public Shared.DTOs.TransactionDto.TransactionDto? ParentTransaction { get; set; }

    public int ChildTransactionId { get; set; }
    public Shared.DTOs.TransactionDto.TransactionDto? ChildTransaction { get; set; }
}