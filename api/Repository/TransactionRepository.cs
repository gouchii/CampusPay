using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;
    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<bool> UpdateTransactionAsync(Transaction transactionModel)
    {
        _context.Transactions.Update(transactionModel);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Transaction?> GetByTransactionRef(string transactionRef)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionRef == transactionRef);
    }
}