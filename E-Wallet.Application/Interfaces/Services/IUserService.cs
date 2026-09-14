using E_Wallet.Application.Common.Result;
using E_Wallet.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result> LoginAsync(LoginDto loginDto);
        Task<Result> RegisterAsync(RegisterDto registerDto);
        Task<Result> RefreshTokenAsync(string refreshToken);

    }
}
