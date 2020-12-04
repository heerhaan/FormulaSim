using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TraitsReworked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Traits",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Traits",
                table: "SeasonTeams");

            migrationBuilder.DropColumn(
                name: "Traits",
                table: "SeasonDrivers");

            migrationBuilder.CreateTable(
                name: "DriverTraits",
                columns: table => new
                {
                    DriverId = table.Column<int>(nullable: false),
                    TraitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverTraits", x => new { x.DriverId, x.TraitId });
                    table.ForeignKey(
                        name: "FK_DriverTraits_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverTraits_Traits_TraitId",
                        column: x => x.TraitId,
                        principalTable: "Traits",
                        principalColumn: "TraitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamTraits",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false),
                    TraitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTraits", x => new { x.TeamId, x.TraitId });
                    table.ForeignKey(
                        name: "FK_TeamTraits_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamTraits_Traits_TraitId",
                        column: x => x.TraitId,
                        principalTable: "Traits",
                        principalColumn: "TraitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackTraits",
                columns: table => new
                {
                    TrackId = table.Column<int>(nullable: false),
                    TraitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackTraits", x => new { x.TrackId, x.TraitId });
                    table.ForeignKey(
                        name: "FK_TrackTraits_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackTraits_Traits_TraitId",
                        column: x => x.TraitId,
                        principalTable: "Traits",
                        principalColumn: "TraitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverTraits_TraitId",
                table: "DriverTraits",
                column: "TraitId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamTraits_TraitId",
                table: "TeamTraits",
                column: "TraitId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTraits_TraitId",
                table: "TrackTraits",
                column: "TraitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverTraits");

            migrationBuilder.DropTable(
                name: "TeamTraits");

            migrationBuilder.DropTable(
                name: "TrackTraits");

            migrationBuilder.AddColumn<string>(
                name: "Traits",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Traits",
                table: "SeasonTeams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Traits",
                table: "SeasonDrivers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
