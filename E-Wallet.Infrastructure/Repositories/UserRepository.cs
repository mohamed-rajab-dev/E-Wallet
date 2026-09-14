using E_Wallet.Application.Common.Result;
using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Repositories
{
    public class UserRepository(UserManager<User> userManager) : IUserRepository
    {
        public UserManager<User> _userManager = userManager;

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }
        public Task<User?> GetUserByIdAsync(long userId)
        {
            return _userManager.FindByIdAsync(userId.ToString());
        }
        public async Task UpdateUserAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
             return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }
    }
}
