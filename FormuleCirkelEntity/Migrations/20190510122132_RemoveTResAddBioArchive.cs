using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class RemoveTResAddBioArchive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamResults");

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Tracks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Teams",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Teams",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Engines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Drivers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Drivers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Engines");

            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Drivers");

            migrationBuilder.CreateTable(
                name: "TeamResults",
                columns: table => new
                {
                    TeamResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EarnedPoints = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false),
                    SeasonTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamResults", x => x.TeamResultId);
                    table.ForeignKey(
                        name: "FK_TeamResults_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamResults_SeasonTeams_SeasonTeamId",
                        column: x => x.SeasonTeamId,
                        principalTable: "SeasonTeams",
                        principalColumn: "SeasonTeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamResults_RaceId",
                table: "TeamResults",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamResults_SeasonTeamId",
                table: "TeamResults",
                column: "SeasonTeamId");
        }
    }
}
