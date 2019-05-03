using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class EnginesNotSeasonBoundRemoveSeasonEngineReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_SeasonEngines_SeasonEngineId",
                table: "SeasonTeams");

            migrationBuilder.DropTable(
                name: "SeasonEngines");

            migrationBuilder.RenameColumn(
                name: "SeasonEngineId",
                table: "SeasonTeams",
                newName: "EngineId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeams_SeasonEngineId",
                table: "SeasonTeams",
                newName: "IX_SeasonTeams_EngineId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_Engines_EngineId",
                table: "SeasonTeams",
                column: "EngineId",
                principalTable: "Engines",
                principalColumn: "EngineId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_Engines_EngineId",
                table: "SeasonTeams");

            migrationBuilder.RenameColumn(
                name: "EngineId",
                table: "SeasonTeams",
                newName: "SeasonEngineId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeams_EngineId",
                table: "SeasonTeams",
                newName: "IX_SeasonTeams_SeasonEngineId");

            migrationBuilder.CreateTable(
                name: "SeasonEngines",
                columns: table => new
                {
                    SeasonEngineId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EngineId = table.Column<int>(nullable: false),
                    Power = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEngines", x => x.SeasonEngineId);
                    table.ForeignKey(
                        name: "FK_SeasonEngines_Engines_EngineId",
                        column: x => x.EngineId,
                        principalTable: "Engines",
                        principalColumn: "EngineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonEngines_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEngines_EngineId",
                table: "SeasonEngines",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEngines_SeasonId",
                table: "SeasonEngines",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_SeasonEngines_SeasonEngineId",
                table: "SeasonTeams",
                column: "SeasonEngineId",
                principalTable: "SeasonEngines",
                principalColumn: "SeasonEngineId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
