using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webthucpham.Data.Migrations
{
    public partial class AddProductImageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 8, 6, 17, 53, 54, 382, DateTimeKind.Local).AddTicks(9151));

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppRole",
                keyColumn: "Id",
                keyValue: new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"),
                column: "ConcurrencyStamp",
                value: "f973c511-718f-4f00-94eb-dc1187e6e83a");

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5daf7797-943a-466f-aaed-44ce34fb8709", "AQAAAAEAACcQAAAAEDrYslnXvdZqjDqC4P38IUylCeozJ5DLQpFGgUOm4AUtHd88mMSzwthuBA8ljmekOg==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 8, 11, 18, 52, 28, 156, DateTimeKind.Local).AddTicks(1074));

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 8, 6, 17, 53, 54, 382, DateTimeKind.Local).AddTicks(9151),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "AppRole",
                keyColumn: "Id",
                keyValue: new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"),
                column: "ConcurrencyStamp",
                value: "a85eb266-01b5-48f2-8e9f-ee35f86a012b");

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4bd77b89-f43b-4c0b-b917-69f6e8fb3952", "AQAAAAEAACcQAAAAEMHkJlEoJ79DKQBjCCx/m4o3fLKttjxOPFD9iIe4zT156fBaYsDeMCP5gKb21RRfAA==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 8, 6, 17, 53, 54, 405, DateTimeKind.Local).AddTicks(3572));
        }
    }
}
