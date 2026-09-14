using System;
using System.Collections.Generic;
using System.Text;
using E_Wallet.Domain.Entities;

namespace E_Wallet.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> CreateJwtToken(User user);
        RefreshToken GenerateRefreshToken();
        void SetRefreshTokenCookie(string refreshToken, DateTimeOffset expires);
        string? GetRefreshTokenFromCookie();
        void DeleteRefreshTokenCookie();
    }
}
