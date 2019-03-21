using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DriverStandings",
                columns: table => new
                {
                    DriverStandingsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Points = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverStandings", x => x.DriverStandingsId);
                });

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    QualyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.QualyId);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Grid = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultId);
                });

            migrationBuilder.CreateTable(
                name: "TeamStandings",
                columns: table => new
                {
                    TeamStandingsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Points = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStandings", x => x.TeamStandingsId);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StatusText = table.Column<string>(nullable: true),
                    ResultId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.StatusId);
                    table.ForeignKey(
                        name: "FK_Statuses_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "ResultId",
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
                    QualyId = table.Column<int>(nullable: false),
                    ResultId = table.Column<int>(nullable: false),
                    DriverStandingsId = table.Column<int>(nullable: false),
                    TeamStandingsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.RaceId);
                    table.ForeignKey(
                        name: "FK_Races_DriverStandings_DriverStandingsId",
                        column: x => x.DriverStandingsId,
                        principalTable: "DriverStandings",
                        principalColumn: "DriverStandingsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_Qualifications_QualyId",
                        column: x => x.QualyId,
                        principalTable: "Qualifications",
                        principalColumn: "QualyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Races_TeamStandings_TeamStandingsId",
                        column: x => x.TeamStandingsId,
                        principalTable: "TeamStandings",
                        principalColumn: "TeamStandingsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    SeasonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.SeasonId);
                    table.ForeignKey(
                        name: "FK_Seasons_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    TrackId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DNFodds = table.Column<int>(nullable: false),
                    RNGodds = table.Column<int>(nullable: false),
                    Specification = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.TrackId);
                    table.ForeignKey(
                        name: "FK_Tracks_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DriverDetails",
                columns: table => new
                {
                    DriverDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Skill = table.Column<int>(nullable: false),
                    DrivingStyle = table.Column<int>(nullable: false),
                    Tires = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverDetails", x => x.DriverDetailId);
                    table.ForeignKey(
                        name: "FK_DriverDetails_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EngineDetails",
                columns: table => new
                {
                    EngineDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Power = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineDetails", x => x.EngineDetailId);
                    table.ForeignKey(
                        name: "FK_EngineDetails_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamDetails",
                columns: table => new
                {
                    TeamDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Chassis = table.Column<int>(nullable: false),
                    Reliability = table.Column<int>(nullable: false),
                    Specification = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDetails", x => x.TeamDetailId);
                    table.ForeignKey(
                        name: "FK_TeamDetails_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    DriverId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DriverNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Abbreviation = table.Column<string>(nullable: true),
                    QualyId = table.Column<int>(nullable: false),
                    ResultId = table.Column<int>(nullable: false),
                    DriverDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_Drivers_DriverDetails_DriverDetailId",
                        column: x => x.DriverDetailId,
                        principalTable: "DriverDetails",
                        principalColumn: "DriverDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Drivers_Qualifications_QualyId",
                        column: x => x.QualyId,
                        principalTable: "Qualifications",
                        principalColumn: "QualyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Drivers_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Engines",
                columns: table => new
                {
                    EngineId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Power = table.Column<int>(nullable: false),
                    EngineDetailId = table.Column<int>(nullable: false),
                    TeamDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.EngineId);
                    table.ForeignKey(
                        name: "FK_Engines_EngineDetails_EngineDetailId",
                        column: x => x.EngineDetailId,
                        principalTable: "EngineDetails",
                        principalColumn: "EngineDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Engines_TeamDetails_TeamDetailId",
                        column: x => x.TeamDetailId,
                        principalTable: "TeamDetails",
                        principalColumn: "TeamDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    QualyId = table.Column<int>(nullable: false),
                    ResultId = table.Column<int>(nullable: false),
                    TeamDetailId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                    table.ForeignKey(
                        name: "FK_Teams_Qualifications_QualyId",
                        column: x => x.QualyId,
                        principalTable: "Qualifications",
                        principalColumn: "QualyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teams_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "ResultId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teams_TeamDetails_TeamDetailId",
                        column: x => x.TeamDetailId,
                        principalTable: "TeamDetails",
                        principalColumn: "TeamDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverDetails_SeasonId",
                table: "DriverDetails",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_DriverDetailId",
                table: "Drivers",
                column: "DriverDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_QualyId",
                table: "Drivers",
                column: "QualyId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_ResultId",
                table: "Drivers",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_EngineDetails_SeasonId",
                table: "EngineDetails",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Engines_EngineDetailId",
                table: "Engines",
                column: "EngineDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Engines_TeamDetailId",
                table: "Engines",
                column: "TeamDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_DriverStandingsId",
                table: "Races",
                column: "DriverStandingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_QualyId",
                table: "Races",
                column: "QualyId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_ResultId",
                table: "Races",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_TeamStandingsId",
                table: "Races",
                column: "TeamStandingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_RaceId",
                table: "Seasons",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_ResultId",
                table: "Statuses",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamDetails_SeasonId",
                table: "TeamDetails",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_QualyId",
                table: "Teams",
                column: "QualyId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ResultId",
                table: "Teams",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamDetailId",
                table: "Teams",
                column: "TeamDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_RaceId",
                table: "Tracks",
                column: "RaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Engines");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "DriverDetails");

            migrationBuilder.DropTable(
                name: "EngineDetails");

            migrationBuilder.DropTable(
                name: "TeamDetails");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "DriverStandings");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "TeamStandings");
        }
    }
}
