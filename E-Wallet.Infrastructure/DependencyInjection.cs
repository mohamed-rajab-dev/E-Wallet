using E_Wallet.Application.Common.Result;
using E_Wallet.Application.Interfaces.Respositories;
using E_Wallet.Application.Interfaces.Services;
using E_Wallet.Domain.Entities;
using E_Wallet.Infrastructure.Persistence;
using E_Wallet.Infrastructure.Repositories;
using E_Wallet.Infrastructure.Services;
using E_Wallet.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Wallet.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                migration => migration.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)

                );

            });

            services.AddIdentity<User, IdentityRole<long>>(options =>
            {
                options.Password.RequiredLength = 1;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();


            services.Configure<Jwt>(config.GetSection("Jwt"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDynamicOtpService, DynamicOtpService>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = config.GetValue<string>("Jwt:Issuer"),
                    ValidAudience = config.GetValue<string>("Jwt:Audience"),
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:Key") ?? "")),
                    ValidateIssuerSigningKey = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(Result.Failure("You are not authenticated"));
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(Result.Failure("You are not authorized to access this resource"));
                    }
                };
            });
            return services;
        }

    }
}
