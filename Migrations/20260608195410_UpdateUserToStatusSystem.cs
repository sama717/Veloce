using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veloco.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserToStatusSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "\"Status\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "\"Status\" = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payment_Total_Matches_Sum",
                table: "Payments",
                sql: "\"TotalAmount\" = (\"Amount\" + \"Tax\")");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Car_Price_Positive",
                table: "Cars",
                sql: "\"Price\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Car_PricePerDay_Positive",
                table: "Cars",
                sql: "\"PricePerDay\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Car_Pricing_Match_ListingType",
                table: "Cars",
                sql: "(\"Type\" = 'Sale' AND \"Price\" IS NOT NULL AND \"PricePerDay\" IS NULL) OR (\"Type\" = 'Rent' AND \"PricePerDay\" IS NOT NULL AND \"Price\" IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_Dates_Valid",
                table: "Bookings",
                sql: "\"EndDate\" >= \"StartDate\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AssetOwnership_ExclusiveOwner",
                table: "AssetOwnerships",
                sql: "(\"UserId\" IS NOT NULL AND \"DealershipId\" IS NULL) OR (\"UserId\" IS NULL AND \"DealershipId\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payment_Total_Matches_Sum",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Car_Price_Positive",
                table: "Cars");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Car_PricePerDay_Positive",
                table: "Cars");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Car_Pricing_Match_ListingType",
                table: "Cars");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_Dates_Valid",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AssetOwnership_ExclusiveOwner",
                table: "AssetOwnerships");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
