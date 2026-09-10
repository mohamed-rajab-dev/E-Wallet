using E_Wallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public long WalletId { get; set; }
        public long UserId { get; set; }
        public TransactionType Type { get; set; }
        public long ReferenceId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Wallet Wallet { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
