using E_Wallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class Wallet
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public decimal Balance { get; private set; }
        public Currency Currency { get; set; } = Currency.EGP;
        public WalletStatus Status { get; set; } = WalletStatus.Active;
        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public ICollection<DynamicOtp> DynamicOtps { get; set; } = new List<DynamicOtp>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            Balance += amount;
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (Balance < amount)
                throw new InvalidOperationException("Insufficient balance.");

            Balance -= amount;
        }
    }
}
