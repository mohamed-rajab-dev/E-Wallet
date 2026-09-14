using E_Wallet.Application.Interfaces.Services;
using E_Wallet.Application.Services;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace E_Wallet.Application
{
    public  static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMapster();
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITransactionService, TransactionService>();

            return services;
        }
    }
}
