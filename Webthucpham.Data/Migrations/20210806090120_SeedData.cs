using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webthucpham.Data.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductInCategories_Categories_CategoryId1",
                table: "ProductInCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInCategories_Product_CategoryId",
                table: "ProductInCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductInCategories_CategoryId1",
                table: "ProductInCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "ProductInCategories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 8, 6, 16, 1, 19, 801, DateTimeKind.Local).AddTicks(6718),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 8, 6, 13, 45, 20, 394, DateTimeKind.Local).AddTicks(8268));

            migrationBuilder.InsertData(
                table: "AppConfigs",
                columns: new[] { "Key", "Value" },
                values: new object[,]
                {
                    { "HomeTile", "This is home page of Web" },
                    { "HomeKeyword", "This is keyword of Web" },
                    { "HomeDescription", "This is desscription of Web" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsShowOnHome", "ParentId", "SortOrder", "Status" },
                values: new object[,]
                {
                    { 1, true, null, 1, 1 },
                    { 2, true, null, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "IsDefault", "Name" },
                values: new object[,]
                {
                    { "vi-VN", true, "Tiếng Việt" },
                    { "en-US", false, "English" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "DateCreated", "OriginalPrice", "Price", "SeoAlias" },
                values: new object[] { 1, new DateTime(2021, 8, 6, 16, 1, 19, 818, DateTimeKind.Local).AddTicks(2226), 100000m, 200000m, null });

            migrationBuilder.InsertData(
                table: "CategoryTranslations",
                columns: new[] { "Id", "CategoryId", "LanguageId", "Name", "SeoAlias", "SeoDescription", "SeoTitle" },
                values: new object[,]
                {
                    { 1, 1, "vi-VN", "Rau Củ", "rau-cu", "Rau củ tươi ngon", "Rau củ tươi ngon" },
                    { 3, 2, "vi-VN", "Thịt", "thit", "Thịt tươi ngon", "Thịt tươi ngon" },
                    { 2, 1, "en-US", "Vegetable", "vegetable", "vegetable", "vegetable" },
                    { 4, 2, "en-US", "Meat", "Meat", "Meat", "Meat" }
                });

            migrationBuilder.InsertData(
                table: "ProductInCategories",
                columns: new[] { "CategoryId", "ProductId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "ProductTranslations",
                columns: new[] { "Id", "Description", "Details", "LanguageId", "Name", "ProductId", "SeoAlias", "SeoDescription", "SeoTitle" },
                values: new object[,]
                {
                    { 1, "Mô tả sản phẩm", "Mô tả sản phẩm", "vi-VN", "Rau Củ", 1, "rau-cu", "Rau củ tươi ngon", "Rau củ tươi ngon" },
                    { 2, "Description of product", "Description of product", "en-US", "Vegetable", 1, "vegetable", "vegetable", "vegetable" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductInCategories_ProductId",
                table: "ProductInCategories",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInCategories_Categories_CategoryId",
                table: "ProductInCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInCategories_Product_ProductId",
                table: "ProductInCategories",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductInCategories_Categories_CategoryId",
                table: "ProductInCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductInCategories_Product_ProductId",
                table: "ProductInCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductInCategories_ProductId",
                table: "ProductInCategories");

            migrationBuilder.DeleteData(
                table: "AppConfigs",
                keyColumn: "Key",
                keyValue: "HomeDescription");

            migrationBuilder.DeleteData(
                table: "AppConfigs",
                keyColumn: "Key",
                keyValue: "HomeKeyword");

            migrationBuilder.DeleteData(
                table: "AppConfigs",
                keyColumn: "Key",
                keyValue: "HomeTile");

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CategoryTranslations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductInCategories",
                keyColumns: new[] { "CategoryId", "ProductId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ProductTranslations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductTranslations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en-US");

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "vi-VN");

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "ProductInCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 8, 6, 13, 45, 20, 394, DateTimeKind.Local).AddTicks(8268),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 8, 6, 16, 1, 19, 801, DateTimeKind.Local).AddTicks(6718));

            migrationBuilder.CreateIndex(
                name: "IX_ProductInCategories_CategoryId1",
                table: "ProductInCategories",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInCategories_Categories_CategoryId1",
                table: "ProductInCategories",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductInCategories_Product_CategoryId",
                table: "ProductInCategories",
                column: "CategoryId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
