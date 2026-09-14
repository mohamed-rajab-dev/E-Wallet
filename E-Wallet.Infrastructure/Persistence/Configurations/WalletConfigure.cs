using E_Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Persistence.Configurations
{
    public class WalletConfigure : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Balance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Currency).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.RowVersion)
            .IsRowVersion();

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Wallet)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DynamicOtps)
                .WithOne(x => x.Wallet)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
