using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class StintModifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Engines_Name",
                table: "Engines");

            migrationBuilder.DropColumn(
                name: "Stints",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "StintResults",
                table: "DriverResults");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Engines",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "StintResults",
                columns: table => new
                {
                    StintResultId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    StintStatus = table.Column<int>(nullable: false),
                    DriverResultId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StintResults", x => x.StintResultId);
                    table.ForeignKey(
                        name: "FK_StintResults_DriverResults_DriverResultId",
                        column: x => x.DriverResultId,
                        principalTable: "DriverResults",
                        principalColumn: "DriverResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stints",
                columns: table => new
                {
                    StintId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(nullable: false),
                    ApplyPitstop = table.Column<bool>(nullable: false),
                    ApplyDriverLevel = table.Column<bool>(nullable: false),
                    ApplyChassisLevel = table.Column<bool>(nullable: false),
                    ApplyEngineLevel = table.Column<bool>(nullable: false),
                    ApplyTireLevel = table.Column<bool>(nullable: false),
                    ApplyQualifyingBonus = table.Column<bool>(nullable: false),
                    ApplyTireWear = table.Column<bool>(nullable: false),
                    ApplyReliability = table.Column<bool>(nullable: false),
                    RNGMaximum = table.Column<int>(nullable: false),
                    RNGMinimum = table.Column<int>(nullable: false),
                    RaceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stints", x => x.StintId);
                    table.ForeignKey(
                        name: "FK_Stints_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "RaceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StintResults_DriverResultId",
                table: "StintResults",
                column: "DriverResultId");

            migrationBuilder.CreateIndex(
                name: "IX_Stints_RaceId",
                table: "Stints",
                column: "RaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StintResults");

            migrationBuilder.DropTable(
                name: "Stints");

            migrationBuilder.AddColumn<string>(
                name: "Stints",
                table: "Races",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Engines",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StintResults",
                table: "DriverResults",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Engines_Name",
                table: "Engines",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
