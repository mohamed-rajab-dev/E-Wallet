using E_Wallet.Application.DTOs;
using E_Wallet.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Wallet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController(ITransactionService transactionService) : BaseController
    {
        private readonly ITransactionService _transactionService = transactionService;

        [Authorize(Roles = "User")]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto, [FromHeader] string idempotencyKey)
        {
            var senderEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _transactionService.Transfer(senderEmail!, idempotencyKey, transferDto);
            return HandleResult(result);
        }

        [Authorize(Roles = "User")]
        [HttpPost("generate/otp")]
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpDto generateOtpDto)
        {
            var result = await _transactionService.GenerateOtp(generateOtpDto);
            return HandleResult(result);
        }

        [HttpPost("atm/operation/withdrawal")]
        public async Task<IActionResult> AtmOperationWithdrawal([FromBody] AtmOperationDto atmOperationDto, [FromHeader] string idempotencyKey, [FromHeader] string atmId, [FromHeader] string bankName)
        {
            var result = await _transactionService.Withdraw(atmOperationDto, idempotencyKey, atmId, bankName);
            return HandleResult(result);
        }

        [HttpPost("atm/operation/deposit")]
        public async Task<IActionResult> AtmOperationDeposit([FromBody] AtmOperationDto atmOperationDto, [FromHeader] string idempotencyKey, [FromHeader] string atmId, [FromHeader] string bankName)
        {
            var result = await _transactionService.Deposit(atmOperationDto, idempotencyKey, atmId, bankName);
            return HandleResult(result);
        }
    }
}
