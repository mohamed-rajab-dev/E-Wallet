using E_Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Persistence.Configurations
{
    public class DynamicOtpConfigure : IEntityTypeConfiguration<DynamicOtp>
    {
        public void Configure(EntityTypeBuilder<DynamicOtp> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.CodeHash).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasOne(x => x.Operation)
                .WithOne(x => x.DynamicOtp)
                .HasForeignKey<AtmOperation>(x => x.DynamicOtpId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
