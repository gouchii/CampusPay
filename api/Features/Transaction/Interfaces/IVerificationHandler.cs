using api.Features.Transaction.Models;
using api.Shared.DTOs.TransactionDto;

namespace api.Features.Transaction.Interfaces;

public interface IVerificationHandler
{
    Task<TransactionDto> VerifyAsync(string transactionRef);
}