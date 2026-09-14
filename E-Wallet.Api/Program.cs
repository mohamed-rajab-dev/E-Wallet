
using E_Wallet.Api.CustomMiddleware;
using E_Wallet.Api.Filters;
using E_Wallet.Application;
using E_Wallet.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Wallet.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddScoped<ValidationFilter>();

            builder.Services.AddControllers(options =>
            {
                options.Filters.AddService<ValidationFilter>();
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.AddSlidingWindowLimiter("login", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.SegmentsPerWindow = 5;
                    limiterOptions.QueueLimit = 0;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            builder.Services.AddOpenApi();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthorization();

            app.UseRateLimiter();


            app.MapControllers();

            app.MapFallback(() =>
            {
                return Results.NotFound(new
                {
                    success = false,
                    message = "Endpoint not found."
                });
            });

            app.Run();
        }
    }
}
