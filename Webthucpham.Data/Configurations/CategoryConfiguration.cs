using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Entities;
using Webthucpham.Data.Enums;

namespace Webthucpham.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categoires").HasKey(c => c.Id);

            builder.Property(c => c.Id).UseIdentityColumn();

            builder.Property(c => c.IsOutstanding).IsRequired();

            builder.Property(c => c.Name).IsRequired();
          

            builder.Property(c => c.Status).HasDefaultValue(Status.Active);


        }
    }
}
