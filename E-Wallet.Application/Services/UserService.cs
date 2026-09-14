using E_Wallet.Application.Common.Result;
using E_Wallet.Application.DTOs;
using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Application.Interfaces.Services;
using E_Wallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Services
{
    public class UserService(IUserRepository userRepository, ITokenService tokenService, IWalletRepository walletRepository, IRefreshTokenRepository refreshTokenRepository ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IWalletRepository _walletRepository = walletRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        public async Task<Result> LoginAsync(LoginDto loginDto)
        {
            if(loginDto == null)
                return Result.Failure("Login data is required");

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null || !await _userRepository.CheckPasswordAsync(user, loginDto.Password))
                return Result.Failure("Invalid credentials" , new Dictionary<string, List<string>> { { "Email", [ "Email or Password is incorrect" ] }, { "Password", [ "Email or Password is incorrect" ] } });

            var accessToken = await _tokenService.CreateJwtToken(user);

            user.RemoveAllRefreshTokens();
            
            var refreshToken = _tokenService.GenerateRefreshToken();
            
            user.AddRefreshToken(refreshToken);

            await _userRepository.UpdateUserAsync(user);

            _tokenService.SetRefreshTokenCookie(refreshToken.Token, DateTimeOffset.Now.AddDays(7));

            return Result.Success("Login successful", new { user.FirstName, user.LastName, user.Email, AccessToken = accessToken });

        }

        public async Task<Result> RegisterAsync(RegisterDto registerDto)
        {
            if(registerDto == null)
                return Result.Failure("Register data is required");
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return Result.Failure("Email already exists", new Dictionary<string, List<string>> { { "Email", [ "Email already exists" ] } });
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,

            };

            Console.WriteLine($"User {user.Id} registered successfully");

            var result = await _userRepository.CreateUserAsync(user, registerDto.Password);

            if(!result.Succeeded)
            {
                var errors = new Dictionary<string, List<string>>();
                foreach (var error in result.Errors)
                {
                    if (!errors.ContainsKey(error.Code))
                        errors[error.Code] = new List<string>();
                    errors[error.Code].Add(error.Description);
                }
                return Result.Failure("User registration failed", errors);
            }
            await _userRepository.AddRoleAsync(user, "User");
            await _walletRepository.CreateWallet(user);

            var accessToken = await _tokenService.CreateJwtToken(user);

            user.RemoveAllRefreshTokens();

            var refreshToken = _tokenService.GenerateRefreshToken();

            user.AddRefreshToken(refreshToken);

            await _userRepository.UpdateUserAsync(user);

            _tokenService.SetRefreshTokenCookie(refreshToken.Token, DateTimeOffset.Now.AddDays(7));




            return Result.Success("User registered successfully", new { user.FirstName, user.LastName, user.Email, AccessToken = accessToken });
        }

        public async Task<Result> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return Result.Failure("Refresh token is required");
           var refreshTokenEntity = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (refreshTokenEntity == null)
                return Result.Failure("Invalid refresh token");

            if (refreshTokenEntity.IsExpired)
                return Result.Failure("Refresh token has expired");

            refreshTokenEntity.Revoke();

            var user = await _userRepository.GetUserByIdAsync(refreshTokenEntity.UserId);

            var newAccessToken = await _tokenService.CreateJwtToken(user!);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user!.AddRefreshToken(newRefreshToken);

            await _userRepository.UpdateUserAsync(user);
            _tokenService.SetRefreshTokenCookie(newRefreshToken.Token, DateTimeOffset.Now.AddDays(7));
            return Result.Success("Token refreshed successfully", new { AccessToken = newAccessToken });
        }

    }
}
