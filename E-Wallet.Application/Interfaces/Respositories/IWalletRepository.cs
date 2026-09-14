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

    }
}
