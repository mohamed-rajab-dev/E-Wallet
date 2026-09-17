using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.DTOs
{
    public class TransferDto
    {
        public string ReceiverEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string SenderPassword { get; set; } = string.Empty;
    }
}
