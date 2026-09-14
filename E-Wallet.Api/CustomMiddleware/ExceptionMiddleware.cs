using E_Wallet.Application.Common.Result;
using FluentValidation;

namespace E_Wallet.Api.CustomMiddleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;


        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("1. Enter ExceptionMiddleware");

            try
            {
                await _next(context);

                Console.WriteLine("2. Request completed successfully");
            }
            catch (ValidationException validationException)
            {
                Console.WriteLine("3. Validation exception caught");

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(x => x.ErrorMessage).ToList()
                    );

                await context.Response.WriteAsJsonAsync(
                    Result.Failure("Validation failed.", errors)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("4. Exception caught: " + ex.Message);

                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                // store the exception details in the logs for debugging purposes
                //_logger.LogError(ex, "An unexpected error occurred.");
                await context.Response.WriteAsJsonAsync(
                    Result.Failure(ex.Message) // 
                );
            }
        }

    }
}
