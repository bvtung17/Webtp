using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webthucpham.Data.Migrations
{
    public partial class buidlai : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRole",
                keyColumn: "Id",
                keyValue: new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"),
                column: "ConcurrencyStamp",
                value: "5bf02700-b85a-4860-ad6a-554094698566");

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "18ed1557-b146-493b-b3b7-a62ac5c9d9a5", "AQAAAAEAACcQAAAAEOtMKPeQXVzH2Vrw50lbSaWGNf6/ev0vsNwZPtr+6Z5V9garUjUAKjGIlfd816fJuA==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 9, 24, 17, 48, 51, 474, DateTimeKind.Local).AddTicks(4762));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRole",
                keyColumn: "Id",
                keyValue: new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"),
                column: "ConcurrencyStamp",
                value: "ecb3261b-4672-4ffa-a038-f5c2bcbdf210");

            migrationBuilder.UpdateData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d79600b3-54cb-4099-afb4-2ff66731a4c6", "AQAAAAEAACcQAAAAEE7vJNYdqg9durbnDv2/cOB7QeoBjah2vDhDtANMZgRDe31XNNpeT2LS12ELQj5JKg==" });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 9, 17, 17, 22, 12, 40, DateTimeKind.Local).AddTicks(174));
        }
    }
}
