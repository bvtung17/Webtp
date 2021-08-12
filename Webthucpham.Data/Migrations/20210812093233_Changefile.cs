using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webthucpham.Data.Migrations
{
    public partial class Changefile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "FileSize",
                table: "ProductImage",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AppRole",
                keyColumn: "Id",
                keyValue: new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"),
                column: "ConcurrencyStamp",
                value: "f6b19544-2a05-42f3-80ba-f36394288c8b");

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1d82a51a-8ebc-42bd-b22a-993cdadc998e", "AQAAAAEAACcQAAAAEIo7dHs5yVYP5wJ10FKOLnk3x9PJ1r9K34qn1JRcOTsP39e2sMTI250g6ban4gaB6A==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 8, 12, 16, 32, 32, 316, DateTimeKind.Local).AddTicks(9834));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FileSize",
                table: "ProductImage",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

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
        }
    }
}
