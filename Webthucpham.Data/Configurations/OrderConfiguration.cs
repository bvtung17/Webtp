using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Entities;

namespace Webthucpham.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {

            builder.ToTable("Orders").HasKey(o => o.Id );
           
            builder.Property(o => o.ShipAddress).IsRequired();
            builder.Property(o => o.ShipName).IsRequired();
            builder.Property(o => o.ShipPhoneNumber).IsRequired();
            builder.Property(o => o.CartId).IsRequired(false);
            builder.HasOne(o => o.Client).WithMany(c => c.Orders).HasForeignKey(o => o.ClientId);
        }
    }
}
