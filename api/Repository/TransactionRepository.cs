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

    public async Task<Transaction?> GetByTransactionRefAsync(string transactionRef)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.TransactionRef == transactionRef);
    }

    public async Task<Transaction?> UpdateAsync(Transaction transactionModel, params string[] updatedProperties)
    {
        var existingTransaction = await _context.Transactions.FindAsync(transactionModel.Id);
        if (existingTransaction == null)
        {
            return null;
        }

        foreach (var property in updatedProperties)
        {
            switch (property)
            {
                case nameof(Transaction.Status):
                    existingTransaction.Status = transactionModel.Status;
                    break;
                case nameof(Transaction.VerificationToken):
                    existingTransaction.VerificationToken = transactionModel.VerificationToken;
                    break;
                case nameof(Transaction.TokenGeneratedAt):
                    existingTransaction.TokenGeneratedAt = transactionModel.TokenGeneratedAt;
                    break;
                case nameof(Transaction.SenderId):
                    existingTransaction.SenderId = transactionModel.SenderId;
                    break;
            }
        }

        await _context.SaveChangesAsync();
        return existingTransaction;
    }

}