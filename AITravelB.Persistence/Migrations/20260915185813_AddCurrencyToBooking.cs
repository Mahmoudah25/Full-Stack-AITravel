using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITravelB.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Bookings");
        }
    }
}
