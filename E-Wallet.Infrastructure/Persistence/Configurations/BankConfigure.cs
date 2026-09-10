using E_Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Persistence.Configurations
{
    public class BankConfigure : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.VerifyUrl).IsRequired().HasMaxLength(200);

            builder.HasMany(x => x.AtmOperations)
                   .WithOne(x => x.Bank)
                   .HasForeignKey(x => x.BankId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
