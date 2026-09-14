using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public Wallet Wallet { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }

                public void RemoveAllRefreshTokens()
        {
            RefreshTokens.Clear();
        }

    }
}
