using api.Data;
using api.Features.Transaction.Interfaces;
using api.Features.Transaction.Models;
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
            .Include(t => t.Sender)   // Ensure Sender is loaded
            .Include(t => t.Receiver) // Ensure Receiver is loaded
            .FirstOrDefaultAsync(t => t.TransactionRef == transactionRef);
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