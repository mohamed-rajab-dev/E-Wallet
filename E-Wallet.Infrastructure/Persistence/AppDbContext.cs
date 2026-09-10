using E_Wallet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace E_Wallet.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<long>, long>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users", "security");
            builder.Entity<IdentityRole<long>>().ToTable("Roles", "security");
            builder.Entity<IdentityUserRole<long>>().ToTable("UserRoles", "security");
            builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims", "security");
            builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins", "security");
            builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims", "security");
            builder.Entity<IdentityUserToken<long>>().ToTable("UserTokens", "security");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
