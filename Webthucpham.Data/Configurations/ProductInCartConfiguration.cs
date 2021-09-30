using Webthucpham.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webthuc
    .Data.Configurations
{
    public class ProductInCartConfiguration : IEntityTypeConfiguration<ProductInCart>
    {
        public void Configure(EntityTypeBuilder<ProductInCart> builder)
        {
            builder.ToTable("ProductInCarts").HasKey(x => new { x.CartId, x.ProductId });
            builder.Property(x => x.ProductName).IsRequired();
            builder.Property(x => x.ProductPrice).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.HasOne(x => x.Product).WithMany(x => x.ProductInCarts).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.Cart).WithMany(x => x.ProductInCarts).HasForeignKey(x => x.CartId);
        }
    }
}
