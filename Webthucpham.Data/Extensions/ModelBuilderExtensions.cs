using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Webthucpham.Data.Entities;
using Webthucpham.Data.Enums;

namespace Webthucpham.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // tạo app config
            modelBuilder.Entity<AppConfig>().HasData(
               new AppConfig() { Key = "HomeTile", Value = "This is home page of Web" },
               new AppConfig() { Key = "HomeKeyword", Value = "This is keyword of Web" },
               new AppConfig() { Key = "HomeDescription", Value = "This is desscription of Web" }
               );
            //tạo ngôn ngữ
            modelBuilder.Entity<Language>().HasData(
                //Ngôn ngữ 1
                new Language() { Id = "vi-VN", Name = "Tiếng Việt", IsDefault = true },
                 // Ngon ngữ 2
                 new Language() { Id = "en-US", Name = "English", IsDefault = false }
                 );
            // tạo Category ( danh mục)
            modelBuilder.Entity<Category>().HasData(
                // danh mục sản phẩm 1
                new Category()
                {
                    Id = 1,
                    IsShowOnHome = true,
                    ParentId = null,
                    SortOrder = 1,
                    Status = Status.Active,


                },
                // danh mục sản phẩm 2
                new Category()
                {
                    Id = 2,
                    IsShowOnHome = true,
                    ParentId = null,
                    SortOrder = 2,
                    Status = Status.Active,
                }
                ); ;
            // tạo CategoryTranslation
            modelBuilder.Entity<CategoryTranslation>().HasData(
                    new CategoryTranslation() // danh mục tiếng việt của rau
                    {
                        Id = 1,
                        CategoryId = 1,
                        Name = "Rau Củ",
                        LanguageId = "vi-VN",
                        SeoAlias = "rau-cu",
                        SeoDescription = "Rau củ tươi ngon",
                        SeoTitle = "Rau củ tươi ngon"
                    },
                      new CategoryTranslation() // danh mục tiếng anh của rau
                      {
                          Id = 2,
                          CategoryId = 1,
                          Name = "Vegetable",
                          LanguageId = "en-US",
                          SeoAlias = "vegetable",
                          SeoDescription = "vegetable",
                          SeoTitle = "vegetable"
                      },
                        new CategoryTranslation() // danh mục tiếng anh của thịt
                        {
                            Id = 3,
                            CategoryId = 2,
                            Name = "Thịt",
                            LanguageId = "vi-VN",
                            SeoAlias = "thit",
                            SeoDescription = "Thịt tươi ngon",
                            SeoTitle = "Thịt tươi ngon"
                        },
                      new CategoryTranslation() // danh mục tiếng anh của thịt
                      {
                          Id = 4,
                          CategoryId = 2,
                          Name = "Meat",
                          LanguageId = "en-US",
                          SeoAlias = "Meat",
                          SeoDescription = "Meat",
                          SeoTitle = "Meat"
                      }

                );

            // tạo Product
            modelBuilder.Entity<Product>().HasData(
             new Product() // product 1 
             {
                 Id = 1,
                 DateCreated = DateTime.Now,
                 OriginalPrice = 100000,
                 Price = 200000,
                 Stock = 0,
                 ViewCount = 0,

             }
             );
            // tạo ProductTransactison : mỗi sản phẩm gồm 2 ngôn ngữ

            modelBuilder.Entity<ProductTranslation>().HasData(
                 new ProductTranslation()  // Sản Phẩm 1
                 {
                     Id=1,
                     ProductId = 1,
                     Name = "Rau Củ",
                     LanguageId = "vi-VN",
                     SeoAlias = "rau-cu",
                     SeoDescription = "Rau củ tươi ngon",
                     SeoTitle = "Rau củ tươi ngon",
                     Details = "Mô tả sản phẩm",
                     Description = "Mô tả sản phẩm"
                 },
                 new ProductTranslation()   // Sản Phẩm 1
                 {
                     Id = 2,
                     ProductId = 1,
                     Name = "Vegetable",
                     LanguageId = "en-US",
                     SeoAlias = "vegetable",
                     SeoDescription = "vegetable",
                     SeoTitle = "vegetable",
                     Details = "Description of product",
                     Description = "Description of product"
                 }
                );
            modelBuilder.Entity<ProductInCategory>().HasData(

            new ProductInCategory() { ProductId = 1, CategoryId = 1 }
             );

        }
    }
}
