using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Services
{
    public interface IDynamicOtpService
    {
        string GenerateOtpAsync();
        bool VerifyOtp(string otp, string storedHash);
        string HashOtp(string otp);
    }
}
