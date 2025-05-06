using System.ComponentModel.DataAnnotations;
using api.Features.Transaction.Enums;

namespace api.Features.Transaction.Helpers;

public class TransactionQueryObject
{
    public TransactionType? Type { get; set; } = null;
    public PaymentMethod? PaymentMethod { get; set; } = null;
    public TransactionStatus? TransactionStatus { get; set; } = null;
    [DataType(DataType.DateTime)] public DateTime? TransactionDate { get; set; } = null;
    public bool? SentTransactions { get; set; } = null;
    public bool? ReceivedTransactions { get; set; } = null;
    public TransactionSortBy? SortBy { get; set; } = null;
    public bool IsDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}