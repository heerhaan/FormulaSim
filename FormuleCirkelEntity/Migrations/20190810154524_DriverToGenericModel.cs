using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class DriverToGenericModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Drivers_MostRecentWinnerDriverId",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "MostRecentWinnerDriverId",
                table: "Tracks",
                newName: "MostRecentWinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tracks_MostRecentWinnerDriverId",
                table: "Tracks",
                newName: "IX_Tracks_MostRecentWinnerId");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Drivers",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Drivers_MostRecentWinnerId",
                table: "Tracks",
                column: "MostRecentWinnerId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Drivers_MostRecentWinnerId",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "MostRecentWinnerId",
                table: "Tracks",
                newName: "MostRecentWinnerDriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Tracks_MostRecentWinnerId",
                table: "Tracks",
                newName: "IX_Tracks_MostRecentWinnerDriverId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Drivers",
                newName: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Drivers_MostRecentWinnerDriverId",
                table: "Tracks",
                column: "MostRecentWinnerDriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
