using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Domain.Entities
{
    public class RefreshToken
    {
        private RefreshToken()
        {
        }

        public RefreshToken(string token, DateTimeOffset expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public RefreshToken(long userId, string token, DateTimeOffset expiresAt)
            : this(token, expiresAt)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }
        public string Token { get; private set; } = null!;
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;

        public User User { get; private set; } = null!;

        public void AssignToUser(long userId)
        {
            UserId = userId;
        }

        public void Revoke()
        {
            RevokedAt = DateTimeOffset.UtcNow;
        }
    }
}
