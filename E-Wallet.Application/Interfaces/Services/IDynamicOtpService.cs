using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Services
{
    public interface IDynamicOtpService
    {
        string GenerateOtpAsync();
        string HashOtp(string otp);
    }
}
