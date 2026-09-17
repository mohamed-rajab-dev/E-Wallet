using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.DTOs
{
    public class AtmOperationDto
    {
        public string Email { get; set; } = string.Empty;
        public string DynamicOtp { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
