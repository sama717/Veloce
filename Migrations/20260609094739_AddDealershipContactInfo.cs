using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veloco.Migrations
{
    /// <inheritdoc />
    public partial class AddDealershipContactInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Dealerships",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Dealerships",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Dealerships");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Dealerships");
        }
    }
}
