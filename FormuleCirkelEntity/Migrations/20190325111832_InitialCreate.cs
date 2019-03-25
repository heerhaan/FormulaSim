using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DriverNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Abbreviation = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverId);
                });

            migrationBuilder.CreateTable(
                name: "Engines",
                columns: table => new
                {
                    EngineId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.EngineId);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    SeasonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.SeasonId);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    TrackId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DNFodds = table.Column<int>(nullable: false),
                    RNGodds = table.Column<int>(nullable: false),
                    Specification = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.TrackId);
                });

            migrationBuilder.CreateTable(
                name: "SeasonEngine",
                columns: table => new
                {
                    SeasonEngineId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Power = table.Column<int>(nullable: false),
                    EngineId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonEngine", x => x.SeasonEngineId);
                    table.ForeignKey(
                        name: "FK_SeasonEngine_Engines_EngineId",
                        column: x => x.EngineId,
                        principalTable: "Engines",
                        principalColumn: "EngineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    RaceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Round = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TrackId = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.RaceId);
                    table.ForeignKey(
                        name: "FK_Races_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "TrackId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeasonTeam",
                columns: table => new
                {
                    SeasonTeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Chassis = table.Column<int>(nullable: false),
                    Relability = table.Column<int>(nullable: false),
                    Specification = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    SeasonEngineId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonTeam", x => x.SeasonTeamId);
                    table.ForeignKey(
                        name: "FK_SeasonTeam_SeasonEngine_SeasonEngineId",
                        column: x => x.SeasonEngineId,
                        principalTable: "SeasonEngine",
                        principalColumn: "SeasonEngineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonTeam_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonTeam_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeasonDriver",
                columns: table => new
                {
                    SeasonDriverId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Skill = table.Column<int>(nullable: false),
                    Style = table.Column<int>(nullable: false),
                    Tires = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: false),
                    SeasonTeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonDriver", x => x.SeasonDriverId);
                    table.ForeignKey(
                        name: "FK_SeasonDriver_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonDriver_SeasonTeam_SeasonTeamId",
                        column: x => x.SeasonTeamId,
                        principalTable: "SeasonTeam",
                        principalColumn: "SeasonTeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamResult",
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
                    table.PrimaryKey("PK_TeamResult", x => x.TeamResultId);
                    table.ForeignKey(
                        name: "FK_TeamResult_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamResult_SeasonTeam_SeasonTeamId",
                        column: x => x.SeasonTeamId,
                        principalTable: "SeasonTeam",
                        principalColumn: "SeasonTeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DriverResult",
                columns: table => new
                {
                    DriverResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Position = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SeasonDriverId = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverResult", x => x.DriverResultId);
                    table.ForeignKey(
                        name: "FK_DriverResult_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverResult_SeasonDriver_SeasonDriverId",
                        column: x => x.SeasonDriverId,
                        principalTable: "SeasonDriver",
                        principalColumn: "SeasonDriverId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    QualyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Position = table.Column<int>(nullable: false),
                    DriverRef = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.QualyId);
                    table.ForeignKey(
                        name: "FK_Qualifications_DriverResult_DriverRef",
                        column: x => x.DriverRef,
                        principalTable: "DriverResult",
                        principalColumn: "DriverResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverResult_RaceId",
                table: "DriverResult",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverResult_SeasonDriverId",
                table: "DriverResult",
                column: "SeasonDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_DriverRef",
                table: "Qualifications",
                column: "DriverRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Races_SeasonId",
                table: "Races",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_TrackId",
                table: "Races",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDriver_DriverId",
                table: "SeasonDriver",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDriver_SeasonTeamId",
                table: "SeasonDriver",
                column: "SeasonTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonEngine_EngineId",
                table: "SeasonEngine",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeam_SeasonEngineId",
                table: "SeasonTeam",
                column: "SeasonEngineId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeam_SeasonId",
                table: "SeasonTeam",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeam_TeamId",
                table: "SeasonTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamResult_RaceId",
                table: "TeamResult",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamResult_SeasonTeamId",
                table: "TeamResult",
                column: "SeasonTeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "TeamResult");

            migrationBuilder.DropTable(
                name: "DriverResult");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "SeasonDriver");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "SeasonTeam");

            migrationBuilder.DropTable(
                name: "SeasonEngine");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Engines");
        }
    }
}
