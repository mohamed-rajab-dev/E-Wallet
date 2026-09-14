using E_Wallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Respositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}
