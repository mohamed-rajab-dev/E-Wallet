using E_Wallet.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace E_Wallet.Infrastructure.Services
{
    public class DynamicOtpService : IDynamicOtpService
    {
        public string GenerateOtpAsync()
        {
            return RandomNumberGenerator
                   .GetInt32(100000, 1000000)
                   .ToString();
        }

        public string HashOtp(string otp)
        {
            var bytes = Encoding.UTF8.GetBytes(otp);

            var hash = SHA256.HashData(bytes);

            return Convert.ToHexString(hash);
        }

        public bool VerifyOtp(string otp, string storedHash)
        {
            var hash = HashOtp(otp);

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(hash),
                Convert.FromHexString(storedHash)
            );
        }
    }
}
