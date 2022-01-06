using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TOMI.Data.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b74ddd14-6340-4840-95c2-db12554843e5"),
                column: "Password",
                value: "AQAAAAEAACcQAAAAEOk4Dt5Xqpivmw4WGF4qpNrC8VzdXoC1IOFTCuueCwu3wYXzHXz+zeyIkjxKPT08+w==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Stores");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b74ddd14-6340-4840-95c2-db12554843e5"),
                column: "Password",
                value: null);
        }
    }
}
