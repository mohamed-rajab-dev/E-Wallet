using E_Wallet.Application.Common.Result;
using E_Wallet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Respositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(long userId);
        Task<User?> GetUserByEmailAsync(string email);

        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task<IdentityResult> AddRoleAsync(User user, string role);

    }
}
