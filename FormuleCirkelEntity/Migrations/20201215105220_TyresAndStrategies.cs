using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TyresAndStrategies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyPitstop",
                table: "Stints");

            migrationBuilder.DropColumn(
                name: "ApplyTireLevel",
                table: "Stints");

            migrationBuilder.DropColumn(
                name: "ApplyTireWear",
                table: "Stints");

            migrationBuilder.DropColumn(
                name: "Tires",
                table: "SeasonDrivers");

            migrationBuilder.AddColumn<int>(
                name: "MaxTyreWear",
                table: "Traits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinTyreWear",
                table: "Traits",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTyreWear",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinTyreWear",
                table: "DriverResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StrategyId",
                table: "DriverResults",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    StrategyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaceLen = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.StrategyId);
                });

            migrationBuilder.CreateTable(
                name: "Tyres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TyreName = table.Column<string>(nullable: true),
                    TyreColour = table.Column<string>(maxLength: 7, nullable: true),
                    StintLen = table.Column<int>(nullable: false),
                    Pace = table.Column<int>(nullable: false),
                    MaxWear = table.Column<int>(nullable: false),
                    MinWear = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tyres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TyreStrategies",
                columns: table => new
                {
                    StrategyId = table.Column<int>(nullable: false),
                    TyreId = table.Column<int>(nullable: false),
                    StintNumberApplied = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TyreStrategies", x => new { x.StrategyId, x.TyreId });
                    table.ForeignKey(
                        name: "FK_TyreStrategies_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "StrategyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TyreStrategies_Tyres_TyreId",
                        column: x => x.TyreId,
                        principalTable: "Tyres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Strategies",
                columns: new[] { "StrategyId", "RaceLen" },
                values: new object[] { 1, 20 });

            migrationBuilder.InsertData(
                table: "Tyres",
                columns: new[] { "Id", "MaxWear", "MinWear", "Pace", "StintLen", "TyreColour", "TyreName" },
                values: new object[] { 1, 0, 0, 0, 20, "#666699", "Grooved" });

            migrationBuilder.InsertData(
                table: "TyreStrategies",
                columns: new[] { "StrategyId", "TyreId", "StintNumberApplied" },
                values: new object[] { 1, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_DriverResults_StrategyId",
                table: "DriverResults",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_TyreStrategies_TyreId",
                table: "TyreStrategies",
                column: "TyreId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverResults_Strategies_StrategyId",
                table: "DriverResults",
                column: "StrategyId",
                principalTable: "Strategies",
                principalColumn: "StrategyId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverResults_Strategies_StrategyId",
                table: "DriverResults");

            migrationBuilder.DropTable(
                name: "TyreStrategies");

            migrationBuilder.DropTable(
                name: "Strategies");

            migrationBuilder.DropTable(
                name: "Tyres");

            migrationBuilder.DropIndex(
                name: "IX_DriverResults_StrategyId",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "MaxTyreWear",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "MinTyreWear",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "MaxTyreWear",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "MinTyreWear",
                table: "DriverResults");

            migrationBuilder.DropColumn(
                name: "StrategyId",
                table: "DriverResults");

            migrationBuilder.AddColumn<bool>(
                name: "ApplyPitstop",
                table: "Stints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyTireLevel",
                table: "Stints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyTireWear",
                table: "Stints",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Tires",
                table: "SeasonDrivers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
