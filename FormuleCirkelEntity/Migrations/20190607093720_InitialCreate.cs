using System;
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
                    Biography = table.Column<string>(nullable: true),
                    Archived = table.Column<bool>(nullable: false)
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
                    Name = table.Column<string>(nullable: true),
                    Power = table.Column<int>(nullable: false),
                    Available = table.Column<bool>(nullable: false),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Engines", x => x.EngineId);
                });

            migrationBuilder.CreateTable(
                name: "Qualification",
                columns: table => new
                {
                    QualyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RaceId = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: false),
                    TeamName = table.Column<string>(nullable: true),
                    Colour = table.Column<string>(maxLength: 7, nullable: true),
                    Accent = table.Column<string>(maxLength: 7, nullable: true),
                    DriverName = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: true),
                    Position = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualification", x => x.QualyId);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    SeasonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SeasonStart = table.Column<DateTime>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    QualificationRemainingDriversQ2 = table.Column<int>(nullable: false),
                    QualificationRemainingDriversQ3 = table.Column<int>(nullable: false)
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
                    Name = table.Column<string>(nullable: true),
                    Abbreviation = table.Column<string>(nullable: true),
                    Colour = table.Column<string>(maxLength: 7, nullable: true),
                    Accent = table.Column<string>(maxLength: 7, nullable: true),
                    Biography = table.Column<string>(nullable: true),
                    Archived = table.Column<bool>(nullable: false)
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
                    Name = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    LengthKM = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DNFodds = table.Column<int>(nullable: false),
                    RNGodds = table.Column<int>(nullable: false),
                    Specification = table.Column<int>(nullable: false),
                    Archived = table.Column<bool>(nullable: false),
                    MostRecentWinnerDriverId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.TrackId);
                    table.ForeignKey(
                        name: "FK_Tracks_Drivers_MostRecentWinnerDriverId",
                        column: x => x.MostRecentWinnerDriverId,
                        principalTable: "Drivers",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeasonTeams",
                columns: table => new
                {
                    SeasonTeamId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Chassis = table.Column<int>(nullable: false),
                    Reliability = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    Specification = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false),
                    EngineId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonTeams", x => x.SeasonTeamId);
                    table.ForeignKey(
                        name: "FK_SeasonTeams_Engines_EngineId",
                        column: x => x.EngineId,
                        principalTable: "Engines",
                        principalColumn: "EngineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonTeams_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "TeamId",
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
                    StintProgress = table.Column<int>(nullable: false),
                    TrackId = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false),
                    Stints = table.Column<string>(nullable: true)
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
                name: "SeasonDrivers",
                columns: table => new
                {
                    SeasonDriverId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Skill = table.Column<int>(nullable: false),
                    Style = table.Column<int>(nullable: false),
                    Tires = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    DriverId = table.Column<int>(nullable: false),
                    SeasonTeamId = table.Column<int>(nullable: false),
                    SeasonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonDrivers", x => x.SeasonDriverId);
                    table.ForeignKey(
                        name: "FK_SeasonDrivers_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "DriverId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonDrivers_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "SeasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeasonDrivers_SeasonTeams_SeasonTeamId",
                        column: x => x.SeasonTeamId,
                        principalTable: "SeasonTeams",
                        principalColumn: "SeasonTeamId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DriverResults",
                columns: table => new
                {
                    DriverResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Points = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Grid = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StintResults = table.Column<string>(nullable: true),
                    SeasonDriverId = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverResults", x => x.DriverResultId);
                    table.ForeignKey(
                        name: "FK_DriverResults_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverResults_SeasonDrivers_SeasonDriverId",
                        column: x => x.SeasonDriverId,
                        principalTable: "SeasonDrivers",
                        principalColumn: "SeasonDriverId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverResults_RaceId",
                table: "DriverResults",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverResults_SeasonDriverId",
                table: "DriverResults",
                column: "SeasonDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_Abbreviation",
                table: "Drivers",
                column: "Abbreviation",
                unique: true,
                filter: "[Abbreviation] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Engines_Name",
                table: "Engines",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Races_SeasonId",
                table: "Races",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_TrackId",
                table: "Races",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDrivers_DriverId",
                table: "SeasonDrivers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDrivers_SeasonId",
                table: "SeasonDrivers",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonDrivers_SeasonTeamId",
                table: "SeasonDrivers",
                column: "SeasonTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeams_EngineId",
                table: "SeasonTeams",
                column: "EngineId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeams_SeasonId",
                table: "SeasonTeams",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonTeams_TeamId",
                table: "SeasonTeams",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Abbreviation",
                table: "Teams",
                column: "Abbreviation",
                unique: true,
                filter: "[Abbreviation] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_MostRecentWinnerDriverId",
                table: "Tracks",
                column: "MostRecentWinnerDriverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverResults");

            migrationBuilder.DropTable(
                name: "Qualification");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "SeasonDrivers");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "SeasonTeams");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Engines");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
