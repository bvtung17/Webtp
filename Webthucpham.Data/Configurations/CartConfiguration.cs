using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Entities;

namespace Webthucpham.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
           
                builder.ToTable("Carts").HasKey(c => c.Id);
                builder.Property(x => x.ClientId).IsRequired(false);
                builder.HasOne(c => c.Client).WithMany(cl => cl.Carts).HasForeignKey(cl => cl.ClientId);
        }
    }
}
