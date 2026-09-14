using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Domain.Entities;
using E_Wallet.Domain.Enums;
using E_Wallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Repositories
{
    public class WalletRepository(AppDbContext context) : IWalletRepository
    {
        private readonly AppDbContext _context = context;
        public async Task CreateWallet(User user)
        {
            await _context.Set<Wallet>().AddAsync(new Wallet { UserId = user.Id });
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet?> GetWalletByUserIdAsync(long userId)
        {
            return await _context.Set<Wallet>().FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task Transfer(Wallet sender, Wallet receiver, decimal amount, string idempotencyKey)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {


                // Check if this request was already processed
                var existingTransaction =
                    await _context.Set<Transaction>()
                        .FirstOrDefaultAsync(x =>
                            x.IdempotencyKey == idempotencyKey);

                if (existingTransaction is not null)
                {
                    return;
                }

                sender.Debit(amount);
                receiver.Credit(amount);



                _context.Set<Transaction>().AddRange(
                [
                    new()
                    {
                         WalletId = sender.Id,
                         UserId = sender.UserId,
                         Type = TransactionType.Transfer,
                         ReferenceId = receiver.Id,
                         Amount = amount,
                         IdempotencyKey = idempotencyKey


                    },
                    new()
                    {
                        WalletId = receiver.Id,
                        UserId = receiver.UserId,
                        Type = TransactionType.Received,
                        ReferenceId = sender.Id,
                        Amount = amount,
                        IdempotencyKey = idempotencyKey
                    }

                ]);


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                throw new InvalidOperationException(
                    "The wallet was modified by another operation. Please try again.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
