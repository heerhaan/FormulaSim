using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class FixSeasonTeamReliabilitySpelling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Relability",
                table: "SeasonTeams",
                newName: "Reliability");

            migrationBuilder.AddColumn<int>(
                name: "SeasonId",
                table: "SeasonEngines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEngines_SeasonId",
                table: "SeasonEngines",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEngines_Seasons_SeasonId",
                table: "SeasonEngines",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "SeasonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEngines_Seasons_SeasonId",
                table: "SeasonEngines");

            migrationBuilder.DropIndex(
                name: "IX_SeasonEngines_SeasonId",
                table: "SeasonEngines");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "SeasonEngines");

            migrationBuilder.RenameColumn(
                name: "Reliability",
                table: "SeasonTeams",
                newName: "Relability");
        }
    }
}