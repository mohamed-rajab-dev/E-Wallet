using E_Wallet.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<Result> Transfer(string senderEmail, string idempotencyKey, TransferDto transferDto);
    }
}
