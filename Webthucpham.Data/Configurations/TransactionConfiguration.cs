using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Entities;

namespace Webthucpham.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions").HasKey(t => t.Id);

            builder.Property(x => x.Id).UseIdentityColumn();

            builder.HasOne(t => t.Client).WithMany(c => c.Transactions).HasForeignKey(t => t.ClientId);
        }
    }
}
