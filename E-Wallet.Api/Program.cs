
using E_Wallet.Infrastructure;
using Microsoft.AspNetCore.Mvc;

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


            builder.Services.AddControllers();
            
            builder.Services.AddOpenApi();

            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


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
