using E_Wallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Respositories
{
    public interface IWalletRepository
    {
        //Task<decimal> GetWalletBalanceAsync(long userId);
        Task CreateWallet(User user);

        Task<Wallet?> GetWalletByUserIdAsync(long userId);

        Task Transfer(Wallet sender, Wallet receiver, decimal amount, string idempotencyKey);
        Task GenerateOtp(Wallet wallet, string hash, decimal amount);
        Task<DynamicOtp?> GetOtpByWalletIdAsync(long walletId, string codeHash);
        Task UpdateDynamicOtp(DynamicOtp dynamicOtp);
        Task Deposit(Wallet wallet, decimal amount, string idempotencyKey, string bankName, string atmId, long otpId);
        Task Withdrawal(Wallet wallet, decimal amount, string idempotencyKey, string bankName, string atmId, long otpId);
        Task<DynamicOtp?> GetDynamicOtpByHashAsync(long walletId, string otp);
        Task<AtmOperation> CreateOpteration(AtmOperation atmOperation);
        Task<Bank?> GetBankByNameAsync(string bankName);
        Task UpdateAtmOperation(AtmOperation atmOperation);
        }
}
