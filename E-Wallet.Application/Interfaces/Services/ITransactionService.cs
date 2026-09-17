using E_Wallet.Application.Common.Result;
using E_Wallet.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<Result> Transfer(string senderEmail, string idempotencyKey, TransferDto transferDto);
        Task<Result> GenerateOtp(GenerateOtpDto generateOtpDto);
        Task<Result> Withdraw(AtmOperationDto atmOperationDto, string idempotencyKey, string atmId, string bankName);
        Task<Result> Deposit(AtmOperationDto atmOperationDto, string idempotencyKey, string atmId, string bankName);
    }
}
