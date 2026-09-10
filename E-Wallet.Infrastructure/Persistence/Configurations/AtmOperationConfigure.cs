using E_Wallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Wallet.Infrastructure.Persistence.Configurations
{
    public class AtmOperationConfigure : IEntityTypeConfiguration<AtmOperation>
    {
        public void Configure(EntityTypeBuilder<AtmOperation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.TerminalId).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TypeOperation).IsRequired();
            builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.CreatedAt).IsRequired();
        }
    }
}
