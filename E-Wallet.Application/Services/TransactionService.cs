using E_Wallet.Application.Common.Result;
using E_Wallet.Application.DTOs;
using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Application.Interfaces.Services;
using E_Wallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Services
{
    public class TransactionService(IWalletRepository walletRepository, IUserRepository userRepository, IDynamicOtpService dynamicOtpService) : ITransactionService
    {
        private readonly IWalletRepository _walletRepository = walletRepository;
        private readonly IUserRepository _userRepository = userRepository;

        private readonly IDynamicOtpService _dynamicOtpService = dynamicOtpService;

        public async Task<Result> GenerateOtp(GenerateOtpDto generateOtpDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(generateOtpDto.Email);
            if (user == null)
                return Result.Failure("User not found.");
            var result = await _userRepository.CheckPasswordAsync(user, generateOtpDto.Password);

            if(!result)
                return Result.Failure("Invalid password.");

            var wallet = await _walletRepository.GetWalletByUserIdAsync(user.Id);
            if (wallet == null)
                return Result.Failure("User's wallet not found.");

            var otp = _dynamicOtpService.GenerateOtpAsync();

            var hash = _dynamicOtpService.HashOtp(otp);

            await _walletRepository.GenerateOtp(wallet, hash, generateOtpDto.Amount);

            return Result.Success("OTP generated successfully.", new { Otp = otp });
        }

        public async Task<Result> Transfer(string senderEmail, string idempotencyKey, TransferDto transferDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(senderEmail) || transferDto == null)
                    return Result.Failure("data is required.");

                var sender = await _userRepository.GetUserByEmailAsync(senderEmail);
                if (sender == null)
                    return Result.Failure("Sender not found.");

                var receiver = await _userRepository.GetUserByEmailAsync(transferDto.ReceiverEmail);
                if (receiver == null)
                    return Result.Failure("Receiver not found.");

                var wallet = await _walletRepository.GetWalletByUserIdAsync(sender.Id);
                if (wallet == null)
                    return Result.Failure("Sender's wallet not found.");

                var receiverWallet = await _walletRepository.GetWalletByUserIdAsync(receiver.Id);
                if (receiverWallet == null)
                    return Result.Failure("Receiver's wallet not found.");

                await _walletRepository.Transfer(wallet, receiverWallet, transferDto.Amount, idempotencyKey);

                return Result.Success("Transfer successful.");
            }
            catch 
            {
                return Result.Failure("An error occurred during the transfer.");
            }
        }

        public async Task<Result> Withdraw(AtmOperationDto atmOperationDto,  string idempotencyKey, string atmId , string bankName)
        {
            try
            {

                var user = await _userRepository.GetUserByEmailAsync(atmOperationDto.Email);

                if (user == null)
                    return Result.Failure("User not found.");

                var wallet = await _walletRepository.GetWalletByUserIdAsync(user.Id);

                if (wallet == null)
                    return Result.Failure("User's wallet not found.");

                var otpHash = _dynamicOtpService.HashOtp(atmOperationDto.DynamicOtp);

                var OtpValid = await _walletRepository.GetDynamicOtpByHashAsync(wallet.Id, otpHash);

                if (OtpValid == null)
                    return Result.Failure("Invalid OTP.");

                if (OtpValid.IsExpired())
                    return Result.Failure("OTP has expired.");

                if (OtpValid.Status == OtpStatus.Used)
                    return Result.Failure("OTP has already been used");

                if (OtpValid.Amount != atmOperationDto.Amount)
                    return Result.Failure("OTP amount does not match the requested withdrawal amount.");

                await _walletRepository.Withdrawal(wallet, atmOperationDto.Amount, idempotencyKey, bankName, atmId, OtpValid.Id);

                OtpValid.MarkAsUsed();
                await _walletRepository.UpdateDynamicOtp(OtpValid);


                return Result.Success("Withdrawal successful.");

            }
            catch
            {
                return Result.Failure("An error occurred during the withdrawal.");
            }
        }
        public async Task<Result> Deposit(AtmOperationDto atmOperationDto,  string idempotencyKey, string atmId , string bankName)
        {
            try
            {
                
                var user = await _userRepository.GetUserByEmailAsync(atmOperationDto.Email);

                if (user == null)
                    return Result.Failure("User not found.");

                var wallet = await _walletRepository.GetWalletByUserIdAsync(user.Id);

                if (wallet == null)
                    return Result.Failure("User's wallet not found.");

                var otpHash = _dynamicOtpService.HashOtp(atmOperationDto.DynamicOtp);

                var OtpValid = await _walletRepository.GetDynamicOtpByHashAsync(wallet.Id, otpHash);

                if (OtpValid == null)
                    return Result.Failure("Invalid OTP.");

                if(OtpValid.IsExpired())
                    return Result.Failure("OTP has expired.");

                if (OtpValid.Status == OtpStatus.Used)
                    return Result.Failure("OTP has already been used");

                if (OtpValid.Amount != atmOperationDto.Amount)
                    return Result.Failure("OTP amount does not match the requested withdrawal amount.");

                await _walletRepository.Deposit(wallet, atmOperationDto.Amount, idempotencyKey, bankName, atmId, OtpValid.Id);

                OtpValid.MarkAsUsed();
                await _walletRepository.UpdateDynamicOtp(OtpValid);


                return Result.Success("Deposit successful.");

            }
            catch
            {
                return Result.Failure("An error occurred during the Deposit.");
            }
        }
    }
}
