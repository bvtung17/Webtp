using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webthucpham.Data.Migrations
{
    public partial class SeedIdentityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 8, 6, 17, 53, 54, 382, DateTimeKind.Local).AddTicks(9151),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 8, 6, 17, 42, 51, 360, DateTimeKind.Local).AddTicks(6625));

            migrationBuilder.InsertData(
                table: "AppRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"), "a85eb266-01b5-48f2-8e9f-ee35f86a012b", "Administrator role", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "AppUser",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Dob", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"), 0, "4bd77b89-f43b-4c0b-b917-69f6e8fb3952", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "bvtung17@gmail.com", true, "Tung", "Bui", false, null, "bvtung17@gmail.com", "admin", "AQAAAAEAACcQAAAAEMHkJlEoJ79DKQBjCCx/m4o3fLKttjxOPFD9iIe4zT156fBaYsDeMCP5gKb21RRfAA==", null, false, "", false, "admin" });

            migrationBuilder.InsertData(
                table: "AppUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"), new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a") });

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 8, 6, 17, 53, 54, 405, DateTimeKind.Local).AddTicks(3572));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppRole",
                keyColumn: "Id",
                keyValue: new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"));

            migrationBuilder.DeleteData(
                table: "AppUser",
                keyColumn: "Id",
                keyValue: new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a"));

            migrationBuilder.DeleteData(
                table: "AppUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("c78e8003-1877-44f1-a4e8-ebfec06c3279"), new Guid("447eb2ae-81ca-4ebf-a4a0-d085def1879a") });

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 8, 6, 17, 42, 51, 360, DateTimeKind.Local).AddTicks(6625),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 8, 6, 17, 53, 54, 382, DateTimeKind.Local).AddTicks(9151));

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 8, 6, 17, 42, 51, 397, DateTimeKind.Local).AddTicks(3704));
        }
    }
}
