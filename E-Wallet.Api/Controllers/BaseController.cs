using E_Wallet.Application.Common.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Wallet.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult(
            Result result,
            int successStatusCode = StatusCodes.Status200OK)
        {
            if (!result.IsSuccess)
                return BadRequest(result);

            return StatusCode(successStatusCode, result);
        }
    }
}
