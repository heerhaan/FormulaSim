using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class InitialConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisqualifyChance = table.Column<int>(nullable: false),
                    MistakeLowerValue = table.Column<int>(nullable: false),
                    MistakeUpperValue = table.Column<int>(nullable: false),
                    RainAdditionalRNG = table.Column<int>(nullable: false),
                    StormAdditionalRNG = table.Column<int>(nullable: false),
                    SunnyEngineMultiplier = table.Column<double>(nullable: false),
                    OvercastEngineMultiplier = table.Column<double>(nullable: false),
                    WetEngineMultiplier = table.Column<double>(nullable: false),
                    RainDriverReliabilityModifier = table.Column<int>(nullable: false),
                    StormDriverReliabilityModifier = table.Column<int>(nullable: false),
                    MistakeAmountRolls = table.Column<int>(nullable: false),
                    ChassisModifierDriverStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppConfig", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppConfig");
        }
    }
}
