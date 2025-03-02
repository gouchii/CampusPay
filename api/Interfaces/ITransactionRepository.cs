using api.Models;

namespace api.Interfaces;

public interface ITransactionRepository
{
    Task<bool> UpdateTransactionAsync(Transaction transaction);
}