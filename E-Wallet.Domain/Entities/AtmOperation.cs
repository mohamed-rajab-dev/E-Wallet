using E_Wallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class AtmOperation
    {
        public long Id { get; set; }

        public long WalletId { get; set; }
        public long DynamicOtpId { get; set; }
        public long BankId { get; set; }

        public string TerminalId { get; set; } = string.Empty;
        public string TypeOperation { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public AtmOperationStatus Status { get; set; } = AtmOperationStatus.Pending;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? CompleteAt { get; set; }

        public Wallet Wallet { get; set; } = null!;
        public Bank Bank { get; set; } = null!;
        public DynamicOtp DynamicOtp { get; set; } = null!;
    }
}
