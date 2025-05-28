using api.Data;
using api.Features.Transaction.Helpers;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;
using api.Shared.Enums.Transaction;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Transaction.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionModel?> CreateAsync(TransactionModel transactionModelModel)
    {
        await _context.Transactions.AddAsync(transactionModelModel);
        await _context.SaveChangesAsync();
        return transactionModelModel;
    }

    public async Task<TransactionModel?> GetByTransactionRefAsync(string transactionRef)
    {
        return await _context.Transactions
            .Include(t => t.Sender) // Ensure Sender is loaded
            .Include(t => t.Receiver) // Ensure Receiver is loaded
            .FirstOrDefaultAsync(t => t.TransactionRef == transactionRef);
    }

    public async Task<List<TransactionModel>> GetAllByUserIdAsync(string userId, TransactionQueryObject query)
    {
        var transactions = _context.Transactions
            .Include(t => t.Sender)
            .Include(t => t.Receiver)
            .Where(t =>
                (query.SentTransactions == true && t.SenderId == userId) ||
                (query.ReceivedTransactions == true && t.ReceiverId == userId) ||
                (query.SentTransactions == null && query.ReceivedTransactions == null &&
                 (t.SenderId == userId || t.ReceiverId == userId)))
            .AsQueryable();

        if (query.Type.HasValue)
            transactions = transactions.Where(t => t.Type == query.Type);

        if (query.PaymentMethod.HasValue)
            transactions = transactions.Where(t => t.Method == query.PaymentMethod);

        if (query.TransactionStatus.HasValue)
            transactions = transactions.Where(t => t.Status == query.TransactionStatus);

        if (query.TransactionDate.HasValue)
        {
            var date = query.TransactionDate.Value.Date;
            transactions = transactions.Where(t => t.CreatedAt.Date == date);
        }

        if (query.SortBy.HasValue)
        {
            transactions = query.SortBy switch
            {
                TransactionSortBy.Amount => query.IsDescending ? transactions.OrderByDescending(t => t.Amount) : transactions.OrderBy(t => t.Amount),
                TransactionSortBy.CreatedAt => query.IsDescending ? transactions.OrderByDescending(t => t.CreatedAt) : transactions.OrderBy(t => t.CreatedAt),
                TransactionSortBy.Status => query.IsDescending ? transactions.OrderByDescending(t => t.Status) : transactions.OrderBy(t => t.Status),
                _ => transactions.OrderByDescending(t => t.CreatedAt)
            };
        }
        else
        {
            transactions = transactions.OrderByDescending(t => t.CreatedAt);
        }

        int skip = (query.PageNumber - 1) * query.PageSize;

        return await transactions
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync();
    }


    public async Task<TransactionModel?> UpdateAsync(TransactionModel transactionModelModel, params string[] updatedProperties)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transactionModelModel.Id);
        if (existingTransaction == null)
        {
            return null;
        }

        foreach (var property in updatedProperties)
        {
            switch (property)
            {
                case nameof(TransactionModel.Status):
                    existingTransaction.Status = transactionModelModel.Status;
                    break;
                case nameof(TransactionModel.VerificationToken):
                    existingTransaction.VerificationToken = transactionModelModel.VerificationToken;
                    break;
                case nameof(TransactionModel.TokenGeneratedAt):
                    existingTransaction.TokenGeneratedAt = transactionModelModel.TokenGeneratedAt;
                    break;
                case nameof(TransactionModel.SenderId):
                    existingTransaction.SenderId = transactionModelModel.SenderId;
                    break;
            }
        }

        await _context.SaveChangesAsync();
        return existingTransaction;
    }
}