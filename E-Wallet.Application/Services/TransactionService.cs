using E_Wallet.Application.Common.Result;
using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Application.Services
{
    public class TransactionService(IWalletRepository walletRepository, IUserRepository userRepository) : ITransactionService
    {
        private readonly IWalletRepository _walletRepository = walletRepository;
        private readonly IUserRepository _userRepository = userRepository;
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
            catch (Exception ex) 
            {
                return Result.Failure("An error occurred during the transfer.");
            }
        }
    }
}
