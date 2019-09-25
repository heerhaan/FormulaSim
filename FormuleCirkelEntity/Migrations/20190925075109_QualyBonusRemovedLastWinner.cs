using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class QualyBonusRemovedLastWinner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Drivers_MostRecentWinnerDriverId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_MostRecentWinnerDriverId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "MostRecentWinnerDriverId",
                table: "Tracks");

            migrationBuilder.AddColumn<int>(
                name: "QualyBonus",
                table: "Seasons",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QualyBonus",
                table: "Seasons");

            migrationBuilder.AddColumn<int>(
                name: "MostRecentWinnerDriverId",
                table: "Tracks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_MostRecentWinnerDriverId",
                table: "Tracks",
                column: "MostRecentWinnerDriverId");

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
