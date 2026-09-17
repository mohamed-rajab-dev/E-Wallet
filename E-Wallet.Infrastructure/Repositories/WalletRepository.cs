using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Application.Interfaces.Services;
using E_Wallet.Domain.Entities;
using E_Wallet.Domain.Enums;
using E_Wallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Text;

namespace E_Wallet.Infrastructure.Repositories
{
    public class WalletRepository(AppDbContext context, IDynamicOtpService dynamicOtpService) : IWalletRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IDynamicOtpService _dynamicOtpService = dynamicOtpService;
        public async Task CreateWallet(User user)
        {
            await _context.Set<Wallet>().AddAsync(new Wallet { UserId = user.Id });
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet?> GetWalletByUserIdAsync(long userId)
        {
            return await _context.Set<Wallet>().FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<AtmOperation> CreateOpteration(AtmOperation atmOperation)
        {
            await _context.Set<AtmOperation>().AddAsync(atmOperation);
            await _context.SaveChangesAsync();
            return atmOperation;
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
                    throw new InvalidOperationException(
                        "This transfer has already been processed.");
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

        public async Task GenerateOtp(Wallet wallet, string hash, decimal amount)
        {
            await _context.Set<DynamicOtp>().AddAsync(new DynamicOtp
            {
                WalletId = wallet.Id,
                CodeHash = hash,
                ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(15),
                Amount = amount
            });
            await _context.SaveChangesAsync();
        }

        public async Task<DynamicOtp?> GetOtpByWalletIdAsync(long walletId , string codeHash)
        {
            return await _context.Set<DynamicOtp>()
                .FirstOrDefaultAsync(x => x.WalletId == walletId && x.CodeHash == codeHash);
        }

        public async Task Withdrawal(Wallet wallet, decimal amount, string idempotencyKey, string bankName, string atmId, long otpId)
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
                    throw new InvalidOperationException(
                        "This transfer has already been processed.");
                }

                wallet.Debit(amount);


                var bank = await _context.Set<Bank>().FirstOrDefaultAsync(x => x.Name == bankName);

                if (bank is null)
                {
                    throw new InvalidOperationException(
                        $"Bank with name '{bankName}' not found.");
                }


                // The bank must check the ATM

                var atmOperation = new AtmOperation
                {
                    WalletId = wallet.Id,
                    Amount = amount,
                    BankId = bank.Id,
                    DynamicOtpId = otpId,
                    TerminalId = atmId,
                    TypeOperation = "Deposit",
                    Status = AtmOperationStatus.Completed,
                    CompleteAt = DateTimeOffset.UtcNow
                };


                atmOperation.Status = AtmOperationStatus.Completed;
                atmOperation.CompleteAt = DateTimeOffset.UtcNow;

                _context.Set<AtmOperation>().Add(atmOperation);

                _context.Set<Transaction>().Add(new()
                {
                    WalletId = wallet.Id,
                    UserId = wallet.UserId,
                    Type = TransactionType.Deposit,
                    ReferenceId = atmOperation.Id,
                    Amount = amount,
                    IdempotencyKey = idempotencyKey

                });

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
        public async Task Deposit(Wallet wallet, decimal amount, string idempotencyKey, string bankName, string atmId, long otpId )
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
                    throw new InvalidOperationException(
                        "This transfer has already been processed.");
                }

                wallet.Credit(amount);


                var bank = await _context.Set<Bank>().FirstOrDefaultAsync(x => x.Name == bankName);

                if(bank is null)
                {
                    throw new InvalidOperationException(
                        $"Bank with name '{bankName}' not found.");
                }


                // The bank must check the ATM

                var atmOperation = new AtmOperation
                {
                    WalletId = wallet.Id,
                    Amount = amount,
                    BankId = bank.Id,
                    DynamicOtpId = otpId,
                    TerminalId = atmId,
                    TypeOperation = "Deposit",
                    Status = AtmOperationStatus.Completed,
                    CompleteAt = DateTimeOffset.UtcNow
                };


                atmOperation.Status = AtmOperationStatus.Completed;
                atmOperation.CompleteAt = DateTimeOffset.UtcNow;

                _context.Set<AtmOperation>().Add(atmOperation);

                _context.Set<Transaction>().Add(new()
                {
                    WalletId = wallet.Id,
                    UserId = wallet.UserId,
                    Type = TransactionType.Deposit,
                    ReferenceId = atmOperation.Id,
                    Amount = amount,
                    IdempotencyKey = idempotencyKey

                });

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

        public async Task UpdateDynamicOtp(DynamicOtp dynamicOtp)
        {
            _context.Set<DynamicOtp>().Update(dynamicOtp);
            await _context.SaveChangesAsync();
        }


        public async Task<DynamicOtp?> GetDynamicOtpByHashAsync(long walletId, string otpHash)
        {

            return await _context.Set<DynamicOtp>()
                .FirstOrDefaultAsync(x => x.CodeHash == otpHash && x.WalletId == walletId);
        }

        public async Task<Bank?> GetBankByNameAsync(string bankName)
        {
            return await _context.Set<Bank>()
                .FirstOrDefaultAsync(x => x.Name == bankName);
        }

        public async Task UpdateAtmOperation(AtmOperation atmOperation)
        {
            _context.Set<AtmOperation>().Update(atmOperation);
            await _context.SaveChangesAsync();
        }
    }
}
