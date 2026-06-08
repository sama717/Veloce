using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veloco.Migrations
{
    /// <inheritdoc />
    public partial class RelationShipFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Dealerships_DealershipId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_DealershipId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DealershipId",
                table: "Cars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DealershipId",
                table: "Cars",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_DealershipId",
                table: "Cars",
                column: "DealershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Dealerships_DealershipId",
                table: "Cars",
                column: "DealershipId",
                principalTable: "Dealerships",
                principalColumn: "Id");
        }
    }
}
