using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITravelB.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addrelationsBokking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ActivityId",
                table: "Bookings",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Activities_ActivityId",
                table: "Bookings",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Activities_ActivityId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ActivityId",
                table: "Bookings");
        }
    }
}
