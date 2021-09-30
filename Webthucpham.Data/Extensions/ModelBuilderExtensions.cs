using Microsoft.AspNetCore.Identity;
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

            // tạo Category ( danh mục)
            modelBuilder.Entity<Category>().HasData(
                  // danh mục sản phẩm 1
                  new Category
                  {
                      Id = 1,
                      IsOutstanding = false,
                      Name = "Rau Ăn Lá",
                      SortOrder = 1,
                      Status = Status.Active,
                      ParentId = 1,
                  },
                 new Category
                 {
                     Id = 2,
                     IsOutstanding = false,
                     Name = "Rau Ăn Củ",
                     SortOrder = 2,
                     Status = Status.Active,
                     ParentId = 1,
                 },

                 new Category
                 {
                     Id = 3,
                     IsOutstanding = true,
                     Name = "Sản phẩm mới",
                     SortOrder = 3,
                     Status = Status.Active
                 },
                  new Category
                  {
                      Id = 4,
                      IsOutstanding = true,
                      Name = "Sản phẩm được yêu thích",
                      SortOrder = 4,
                      Status = Status.Active
                  },
                 new Category
                 {
                     Id = 5,
                     IsOutstanding = true,
                     Name = "Sản phẩm khuyến mại",
                     SortOrder = 5,
                     Status = Status.Active
                 },
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

             }));
            modelBuilder.Entity<Product>().HasData(
            // Mercedes-Benz
            new Product
            {

                Name = "Bông Cải Trắng Đà Lạt Vietgap ",
               
                Description = "" +
            " Bông Cải Trắng Đà Lạt Vietgap ",
                Details = "Bông 600G",
                OriginalPrice = 39000,
                Stock = 100,
                ViewCount = 0,
                Price = 49000,
                Id = 1,
                OriginalCountry = "Việt Nam",
                status = Status.Active
            },
            new Product
            {

                Name = "Bông Cải Xanh Baby Broccolini Nhập Khẩu Úc  ",
              
                Description =
            " Bông Cải Xanh Baby Broccolini Nhập Khẩu Úc ",
                Details = "Bông 200g",
                OriginalPrice = 69000,
                Stock = 100,
                ViewCount = 0,
                Price = 89000,
                Id = 2,
                OriginalCountry = "Úc",
                status = Status.Active
            },
            new Product
            {
                Name = "VIETRAT- RAU DỀN HƯỚNG HỮU CƠ 300G",
               
                Description = "" +
                " Bông Cải Trắng Đà Lạt Vietgap ",
                Details = "Túi 300G",
                OriginalPrice = 39000,
                Stock = 100,
                ViewCount = 0,
                Price = 49000,
                Id = 3,
                OriginalCountry = "Việt Nam",
                status = Status.Active
            });
            modelBuilder.Entity<ProductInCategory>().HasData(
               new ProductInCategory() { ProductId = 1, CategoryId = 1 },
               new ProductInCategory() { ProductId = 2, CategoryId = 1 },
               new ProductInCategory() { ProductId = 3, CategoryId = 1 });
           modelBuilder.Entity<ProductImage>().HasData(
               // Product Mercedes Benz
               new ProductImage()
               {
                   Id = 1,
                   Caption = "Bông cải",
                   DateCreated = DateTime.Now,
                   FileSize = 123,
                   ImagePath = "1.jpg",
                   ProductId = 1,
                   IsDefault = true,
                   SortOrder = 1
               },
               new ProductImage()
               {
                   Id = 2,
                   Caption = "bông cải",
                   DateCreated = DateTime.Now,
                   FileSize = 123,
                   ImagePath = "2.jpg",
                   ProductId = 2,
                   IsDefault = true,
                   SortOrder = 2
               },
               new ProductImage()
               {
                   Id = 3,
                   Caption = "Rau dền",
                   DateCreated = DateTime.Now,
                   FileSize = 123,
                   ImagePath = "3.jpg",
                   ProductId = 3,
                   IsDefault = true,
                   SortOrder = 3
               });
            // tao admin voi Identity
            var roleID1 = new Guid("C78E8003-1877-44F1-A4E8-EBFEC06C3279");
            var roleID2 = new Guid("C78E8003-2000-44F1-A4E8-EBFEC06C3279");
            var adminID = new Guid("447EB2AE-81CA-4EBF-A4A0-D085DEF1879A");
            var staffID = new Guid("447EB2AE-2000-4EBF-A4A0-D085DEF1239A");
            var clientID = new Guid("17F617A6-DB8F-4D13-8E88-15D9A7AB7927");
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = adminID,
                Name = "admin",
                NormalizedName = "admin",
                Description = "Administrator role"
            },
             new Role()
             {
                 Id = staffID,
                 Name = "Staff",
                 NormalizedName = "Staff",
                 Description = "Staff role"
             });

            var hasher = new PasswordHasher<User>();
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminID,
                UserName = "admin",
                NormalizedUserName = "Quản Trị Viên",
                Email = "bvtung17@gmail.com",
                NormalizedEmail = "bvtung17@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Abcdxyz1@"),
                SecurityStamp = string.Empty,
                Name = "Tùng",
                Dob = new DateTime(2000, 1, 1)
            });

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    RoleId = roleID1,
                    UserId = adminID
                },
                 new IdentityUserRole<Guid>
                 {
                     RoleId = roleID2,
                     UserId = staffID
                 });

            modelBuilder.Entity<Order>().HasData(
                // Client 1
                new Order()
                {
                    ClientId = clientID,
                    Id = 1,
                    ShipEmail = "bvtung17@gmail.com",
                    ShipName = "TEST",
                    ShipPhoneNumber = "0123456",
                    Status = OrderStatus.Success,
                    ShipAddress = "279 NGUYỄN TRI PHƯƠNG - Phường 5 - Quận 10 - Hồ Chí Minh",
                    OrderDate = new DateTime(2021, 08, 27, 4, 0, 0, DateTimeKind.Utc),
                    Price = 1000000
                });
            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail()
                {
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    Price = 1000000
                });
            modelBuilder.Entity<Slider>().HasData(
               new Slider
               {
                   Id = 1,
                   Name = "First Slide",
                   SortOrder = 1,
                   Status = Status.Active,
                   Url = "#",
                   Image = "s1.jpg"
               },
                new Slider
                {
                    Id = 2,
                    Name = "Second Slide",
                    SortOrder = 2,
                    Status = Status.Active,
                    Url = "#",
                    Image = "s2.jpg"
                },
                new Slider
                {
                    Id = 3,
                    Name = "Third Slide",
                    SortOrder = 3,
                    Status = Status.Active,
                    Url = "#",
                    Image = "s1.jpg"
                },
                 new Slider
                 {
                     Id = 4,
                     Name = "Fourth Slide",
                     SortOrder = 4,
                     Status = Status.Active,
                     Url = "#",
                     Image = "s2.jpg"
                 });

        }
    }
}
