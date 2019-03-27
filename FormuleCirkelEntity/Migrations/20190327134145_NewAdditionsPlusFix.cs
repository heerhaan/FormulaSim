using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class NewAdditionsPlusFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverResult_Races_RaceId",
                table: "DriverResult");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverResult_SeasonDriver_SeasonDriverId",
                table: "DriverResult");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualifications_DriverResult_DriverRef",
                table: "Qualifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDriver_Drivers_DriverId",
                table: "SeasonDriver");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDriver_SeasonTeam_SeasonTeamId",
                table: "SeasonDriver");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEngine_Engines_EngineId",
                table: "SeasonEngine");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeam_SeasonEngine_SeasonEngineId",
                table: "SeasonTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeam_Seasons_SeasonId",
                table: "SeasonTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeam_Teams_TeamId",
                table: "SeasonTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamResult_Races_RaceId",
                table: "TeamResult");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamResult_SeasonTeam_SeasonTeamId",
                table: "TeamResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamResult",
                table: "TeamResult");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeasonTeam",
                table: "SeasonTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeasonEngine",
                table: "SeasonEngine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeasonDriver",
                table: "SeasonDriver");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverResult",
                table: "DriverResult");

            migrationBuilder.RenameTable(
                name: "TeamResult",
                newName: "TeamResults");

            migrationBuilder.RenameTable(
                name: "SeasonTeam",
                newName: "SeasonTeams");

            migrationBuilder.RenameTable(
                name: "SeasonEngine",
                newName: "SeasonEngines");

            migrationBuilder.RenameTable(
                name: "SeasonDriver",
                newName: "SeasonDrivers");

            migrationBuilder.RenameTable(
                name: "DriverResult",
                newName: "DriverResults");

            migrationBuilder.RenameIndex(
                name: "IX_TeamResult_SeasonTeamId",
                table: "TeamResults",
                newName: "IX_TeamResults_SeasonTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamResult_RaceId",
                table: "TeamResults",
                newName: "IX_TeamResults_RaceId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeam_TeamId",
                table: "SeasonTeams",
                newName: "IX_SeasonTeams_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeam_SeasonId",
                table: "SeasonTeams",
                newName: "IX_SeasonTeams_SeasonId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeam_SeasonEngineId",
                table: "SeasonTeams",
                newName: "IX_SeasonTeams_SeasonEngineId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonEngine_EngineId",
                table: "SeasonEngines",
                newName: "IX_SeasonEngines_EngineId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonDriver_SeasonTeamId",
                table: "SeasonDrivers",
                newName: "IX_SeasonDrivers_SeasonTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonDriver_DriverId",
                table: "SeasonDrivers",
                newName: "IX_SeasonDrivers_DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverResult_SeasonDriverId",
                table: "DriverResults",
                newName: "IX_DriverResults_SeasonDriverId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverResult_RaceId",
                table: "DriverResults",
                newName: "IX_DriverResults_RaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamResults",
                table: "TeamResults",
                column: "TeamResultId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeasonTeams",
                table: "SeasonTeams",
                column: "SeasonTeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeasonEngines",
                table: "SeasonEngines",
                column: "SeasonEngineId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeasonDrivers",
                table: "SeasonDrivers",
                column: "SeasonDriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverResults",
                table: "DriverResults",
                column: "DriverResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverResults_Races_RaceId",
                table: "DriverResults",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "RaceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverResults_SeasonDrivers_SeasonDriverId",
                table: "DriverResults",
                column: "SeasonDriverId",
                principalTable: "SeasonDrivers",
                principalColumn: "SeasonDriverId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualifications_DriverResults_DriverRef",
                table: "Qualifications",
                column: "DriverRef",
                principalTable: "DriverResults",
                principalColumn: "DriverResultId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDrivers_Drivers_DriverId",
                table: "SeasonDrivers",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDrivers_SeasonTeams_SeasonTeamId",
                table: "SeasonDrivers",
                column: "SeasonTeamId",
                principalTable: "SeasonTeams",
                principalColumn: "SeasonTeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEngines_Engines_EngineId",
                table: "SeasonEngines",
                column: "EngineId",
                principalTable: "Engines",
                principalColumn: "EngineId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_SeasonEngines_SeasonEngineId",
                table: "SeasonTeams",
                column: "SeasonEngineId",
                principalTable: "SeasonEngines",
                principalColumn: "SeasonEngineId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_Seasons_SeasonId",
                table: "SeasonTeams",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "SeasonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeams_Teams_TeamId",
                table: "SeasonTeams",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamResults_Races_RaceId",
                table: "TeamResults",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "RaceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamResults_SeasonTeams_SeasonTeamId",
                table: "TeamResults",
                column: "SeasonTeamId",
                principalTable: "SeasonTeams",
                principalColumn: "SeasonTeamId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverResults_Races_RaceId",
                table: "DriverResults");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverResults_SeasonDrivers_SeasonDriverId",
                table: "DriverResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Qualifications_DriverResults_DriverRef",
                table: "Qualifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDrivers_Drivers_DriverId",
                table: "SeasonDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonDrivers_SeasonTeams_SeasonTeamId",
                table: "SeasonDrivers");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonEngines_Engines_EngineId",
                table: "SeasonEngines");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_SeasonEngines_SeasonEngineId",
                table: "SeasonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_Seasons_SeasonId",
                table: "SeasonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_SeasonTeams_Teams_TeamId",
                table: "SeasonTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamResults_Races_RaceId",
                table: "TeamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamResults_SeasonTeams_SeasonTeamId",
                table: "TeamResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamResults",
                table: "TeamResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeasonTeams",
                table: "SeasonTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeasonEngines",
                table: "SeasonEngines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeasonDrivers",
                table: "SeasonDrivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverResults",
                table: "DriverResults");

            migrationBuilder.RenameTable(
                name: "TeamResults",
                newName: "TeamResult");

            migrationBuilder.RenameTable(
                name: "SeasonTeams",
                newName: "SeasonTeam");

            migrationBuilder.RenameTable(
                name: "SeasonEngines",
                newName: "SeasonEngine");

            migrationBuilder.RenameTable(
                name: "SeasonDrivers",
                newName: "SeasonDriver");

            migrationBuilder.RenameTable(
                name: "DriverResults",
                newName: "DriverResult");

            migrationBuilder.RenameIndex(
                name: "IX_TeamResults_SeasonTeamId",
                table: "TeamResult",
                newName: "IX_TeamResult_SeasonTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamResults_RaceId",
                table: "TeamResult",
                newName: "IX_TeamResult_RaceId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeams_TeamId",
                table: "SeasonTeam",
                newName: "IX_SeasonTeam_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeams_SeasonId",
                table: "SeasonTeam",
                newName: "IX_SeasonTeam_SeasonId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonTeams_SeasonEngineId",
                table: "SeasonTeam",
                newName: "IX_SeasonTeam_SeasonEngineId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonEngines_EngineId",
                table: "SeasonEngine",
                newName: "IX_SeasonEngine_EngineId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonDrivers_SeasonTeamId",
                table: "SeasonDriver",
                newName: "IX_SeasonDriver_SeasonTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_SeasonDrivers_DriverId",
                table: "SeasonDriver",
                newName: "IX_SeasonDriver_DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverResults_SeasonDriverId",
                table: "DriverResult",
                newName: "IX_DriverResult_SeasonDriverId");

            migrationBuilder.RenameIndex(
                name: "IX_DriverResults_RaceId",
                table: "DriverResult",
                newName: "IX_DriverResult_RaceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamResult",
                table: "TeamResult",
                column: "TeamResultId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeasonTeam",
                table: "SeasonTeam",
                column: "SeasonTeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeasonEngine",
                table: "SeasonEngine",
                column: "SeasonEngineId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeasonDriver",
                table: "SeasonDriver",
                column: "SeasonDriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverResult",
                table: "DriverResult",
                column: "DriverResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverResult_Races_RaceId",
                table: "DriverResult",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "RaceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverResult_SeasonDriver_SeasonDriverId",
                table: "DriverResult",
                column: "SeasonDriverId",
                principalTable: "SeasonDriver",
                principalColumn: "SeasonDriverId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Qualifications_DriverResult_DriverRef",
                table: "Qualifications",
                column: "DriverRef",
                principalTable: "DriverResult",
                principalColumn: "DriverResultId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDriver_Drivers_DriverId",
                table: "SeasonDriver",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonDriver_SeasonTeam_SeasonTeamId",
                table: "SeasonDriver",
                column: "SeasonTeamId",
                principalTable: "SeasonTeam",
                principalColumn: "SeasonTeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonEngine_Engines_EngineId",
                table: "SeasonEngine",
                column: "EngineId",
                principalTable: "Engines",
                principalColumn: "EngineId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeam_SeasonEngine_SeasonEngineId",
                table: "SeasonTeam",
                column: "SeasonEngineId",
                principalTable: "SeasonEngine",
                principalColumn: "SeasonEngineId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeam_Seasons_SeasonId",
                table: "SeasonTeam",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "SeasonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SeasonTeam_Teams_TeamId",
                table: "SeasonTeam",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamResult_Races_RaceId",
                table: "TeamResult",
                column: "RaceId",
                principalTable: "Races",
                principalColumn: "RaceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamResult_SeasonTeam_SeasonTeamId",
                table: "TeamResult",
                column: "SeasonTeamId",
                principalTable: "SeasonTeam",
                principalColumn: "SeasonTeamId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
