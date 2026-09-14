using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Wallet.Api.Filters
{
    public sealed class ValidationFilter : IAsyncActionFilter
    {

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                    continue;

                var requestType = argument.GetType();

                var validatorType = typeof(IValidator<>)
                    .MakeGenericType(requestType);

                if (context.HttpContext.RequestServices
                    .GetService(validatorType) is not IValidator validator)
                    continue;

                var validationContext =
                    new ValidationContext<object>(argument);

                var validationResult =
                    await validator.ValidateAsync(
                        validationContext,
                        context.HttpContext.RequestAborted);

                if (!validationResult.IsValid)
                {
                    throw new ValidationException(
                        validationResult.Errors);
                }
            }

            await next();
        }
    }
}
