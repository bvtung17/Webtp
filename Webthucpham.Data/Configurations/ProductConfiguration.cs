using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Entities;

namespace Webthucpham.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.OriginalPrice).IsRequired(); 
            builder.Property(x => x.Stock).IsRequired().HasDefaultValue(0); // giá trị mặc định = 0
            builder.Property(x => x.ViewCount).IsRequired().HasDefaultValue(0);
            builder.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x=>x.Category);

        }
    }
}
