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

    public async Task<Transaction?> CreateAsync(Transaction transactionModel)
    {
        await _context.Transactions.AddAsync(transactionModel);
        await _context.SaveChangesAsync();
        return transactionModel;
    }

    public async Task<Transaction?> UpdateStatusAsync(Transaction transactionModel)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transactionModel.Id);
        if (existingTransaction == null)
        {
            return null;
        }
        existingTransaction.Status = transactionModel.Status;
        await _context.SaveChangesAsync();
        return transactionModel;
    }

    public async Task<Transaction?> UpdateTokenAsync(Transaction transactionModel)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transactionModel.Id);
        if (existingTransaction == null)
        {
            return null;
        }
        existingTransaction.VerificationToken = transactionModel.VerificationToken;
        await _context.SaveChangesAsync();
        return transactionModel;
    }

    public async Task<Transaction?> UpdateTokenTimeAsync(Transaction transactionModel)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transactionModel.Id);
        if (existingTransaction == null)
        {
            return null;
        }
        existingTransaction.TokenGeneratedAt = transactionModel.TokenGeneratedAt;
        await _context.SaveChangesAsync();
        return transactionModel;
    }

    public async Task<Transaction?> UpdateSenderAsync(Transaction transactionModel)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transactionModel.Id);
        if (existingTransaction == null)
        {
            return null;
        }
        existingTransaction.SenderId = transactionModel.SenderId;
        await _context.SaveChangesAsync();
        return transactionModel;
    }

    public async Task<Transaction?> GetByTransactionRefAsync(string transactionRef)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionRef == transactionRef);
    }
}