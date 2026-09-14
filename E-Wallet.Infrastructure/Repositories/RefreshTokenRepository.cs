using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Domain.Entities;
using E_Wallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>().FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.Set<RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
