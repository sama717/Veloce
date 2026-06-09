using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veloco.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityAndConsultationDealership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DealershipId",
                table: "ConsultationDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "Cars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationDetails_DealershipId",
                table: "ConsultationDetails",
                column: "DealershipId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Car_AvailableQuantity_Valid",
                table: "Cars",
                sql: "\"AvailableQuantity\" >= 0 AND \"AvailableQuantity\" <= \"Quantity\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Car_Quantity_Positive",
                table: "Cars",
                sql: "\"Quantity\" > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationDetails_Dealerships_DealershipId",
                table: "ConsultationDetails",
                column: "DealershipId",
                principalTable: "Dealerships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationDetails_Dealerships_DealershipId",
                table: "ConsultationDetails");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationDetails_DealershipId",
                table: "ConsultationDetails");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Car_AvailableQuantity_Valid",
                table: "Cars");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Car_Quantity_Positive",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DealershipId",
                table: "ConsultationDetails");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "Cars");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "Cars",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
