using Microsoft.EntityFrameworkCore;
using E_Wallet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Wallet.Infrastructure.Persistence.Configurations
{
    public class TransactionConfigure : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CreatedAt).IsRequired();

        }
    }
}
