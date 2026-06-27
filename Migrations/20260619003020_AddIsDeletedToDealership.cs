using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veloco.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToDealership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Dealerships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dealerships",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Dealerships");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dealerships");
        }
    }
}
