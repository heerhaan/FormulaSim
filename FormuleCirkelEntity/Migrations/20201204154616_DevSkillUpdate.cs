using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class DevSkillUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeDevRanges",
                table: "Championships");

            migrationBuilder.DropColumn(
                name: "SkillDevRanges",
                table: "Championships");

            migrationBuilder.CreateTable(
                name: "MinMaxDevRange",
                columns: table => new
                {
                    MinMaxDevId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValueKey = table.Column<int>(nullable: false),
                    MinDev = table.Column<int>(nullable: false),
                    MaxDev = table.Column<int>(nullable: false),
                    ChampionshipId = table.Column<int>(nullable: true),
                    ChampionshipId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinMaxDevRange", x => x.MinMaxDevId);
                    table.ForeignKey(
                        name: "FK_MinMaxDevRange_Championships_ChampionshipId",
                        column: x => x.ChampionshipId,
                        principalTable: "Championships",
                        principalColumn: "ChampionshipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MinMaxDevRange_Championships_ChampionshipId1",
                        column: x => x.ChampionshipId1,
                        principalTable: "Championships",
                        principalColumn: "ChampionshipId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MinMaxDevRange_ChampionshipId",
                table: "MinMaxDevRange",
                column: "ChampionshipId");

            migrationBuilder.CreateIndex(
                name: "IX_MinMaxDevRange_ChampionshipId1",
                table: "MinMaxDevRange",
                column: "ChampionshipId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinMaxDevRange");

            migrationBuilder.AddColumn<string>(
                name: "AgeDevRanges",
                table: "Championships",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkillDevRanges",
                table: "Championships",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
