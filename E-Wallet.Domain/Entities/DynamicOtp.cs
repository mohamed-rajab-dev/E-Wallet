using E_Wallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class DynamicOtp
    {
        public long Id { get; set; }
        public long WalletId { get; set; }
        public string CodeHash { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public DateTimeOffset? UsedAt { get; set; }
        public OtpStatus Status { get; set; } = OtpStatus.Pending;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public Wallet Wallet { get; set; } = null!;
        public AtmOperation? Operation { get; set; }

        public bool IsExpired()
        {
            if(DateTimeOffset.UtcNow > ExpirationTime)
            {
                Status = OtpStatus.Expired;
                return true;
            }
            return false;
        }

        public void MarkAsUsed()
        {
            Status = OtpStatus.Used;
            UsedAt = DateTimeOffset.UtcNow;
        }
    }
}
