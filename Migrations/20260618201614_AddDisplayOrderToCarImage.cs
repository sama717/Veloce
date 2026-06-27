using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veloco.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayOrderToCarImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "CarImages",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "CarImages");
        }
    }
}
