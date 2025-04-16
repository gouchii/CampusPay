using api.Features.Auth.Models;
using api.Features.Transaction.Models;
using api.Features.User;
using api.Features.Wallet;
using api.Shared.Auth.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class AppDbContext : IdentityDbContext<UserModel>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<WalletModel> Wallets { get; set; }
    public DbSet<TransactionModel> Transactions { get; set; }
    public DbSet<TransactionRelationModel> TransactionRelations { get; set; }
    public DbSet<RefreshTokenModel> RefreshTokens { get; set; }
    public DbSet<UserCredentialModel> UserCredentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User - Wallet: One-to-Many
        modelBuilder.Entity<WalletModel>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TransactionModel>()
            .HasOne(t => t.Sender)
            .WithMany(u => u.SentTransactions)
            .HasForeignKey(t => t.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TransactionModel>()
            .HasOne(t => t.Receiver)
            .WithMany(u => u.ReceivedTransactions)
            .HasForeignKey(t => t.ReceiverId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TransactionRelationModel>()
            .HasKey(tr => new { tr.ParentTransactionId, tr.ChildTransactionId });

        modelBuilder.Entity<TransactionRelationModel>()
            .HasOne(tr => tr.ParentTransaction)
            .WithMany(t => t.ChildRelations)
            .HasForeignKey(tr => tr.ParentTransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TransactionRelationModel>()
            .HasOne(tr => tr.ChildTransaction)
            .WithMany(t => t.ParentRelations)
            .HasForeignKey(tr => tr.ChildTransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<RefreshTokenModel>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TransactionModel>()
            .HasIndex(t => t.TransactionRef)
            .IsUnique();

        modelBuilder.Entity<RefreshTokenModel>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // modelBuilder.Entity<UserModel>()
        //     .HasIndex(u => u.RfidTag)
        //     .IsUnique();

        modelBuilder.Entity<UserModel>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<TransactionModel>()
            .Property(t => t.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<WalletModel>()
            .Property(w => w.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<RefreshTokenModel>()
            .Property(rt => rt.CreatedAt)
            .HasDefaultValueSql("GETDATE()");


        //todo copy this pattern
        modelBuilder.Entity<UserCredentialModel>(entity =>
        {
            entity.HasKey(uc => uc.Id);

            entity.Property(uc => uc.Type)
                .HasConversion<string>() // Store enum as string
                .IsRequired();

            entity.Property(uc => uc.HashedValue)
                .IsRequired();

            entity.Property(uc => uc.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(uc => uc.User)
                .WithMany(u => u.UserCredentials)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(uc => new { uc.UserId, uc.Type })
                .IsUnique(); // One credential type per user

            // entity.HasIndex(uc => uc.Type)
            //     .IsUnique(); // Indexes the enum field itself
            entity.HasIndex(uc => uc.UserId) // assuming RfidTag is not a property here
                .IsUnique()
                .HasFilter("[Type] = 'RfidTag'"); // Unique rfid tag per user
        });

        // **Seeding Default Roles**
        List<IdentityRole> roles =
        [
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            },

            new IdentityRole
            {
                Name = "Merchant",
                NormalizedName = "MERCHANT"
            },

            new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER"
            }
        ];

        modelBuilder.Entity<IdentityRole>().HasData(roles);
    }
}