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
    }
}
