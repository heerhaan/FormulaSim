using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class IdedStrategies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TyreStrategies",
                table: "TyreStrategies");

            migrationBuilder.DeleteData(
                table: "TyreStrategies",
                keyColumns: new[] { "StrategyId", "TyreId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.AddColumn<int>(
                name: "TyreStrategyId",
                table: "TyreStrategies",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TyreStrategies",
                table: "TyreStrategies",
                column: "TyreStrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_TyreStrategies_StrategyId",
                table: "TyreStrategies",
                column: "StrategyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TyreStrategies",
                table: "TyreStrategies");

            migrationBuilder.DropIndex(
                name: "IX_TyreStrategies_StrategyId",
                table: "TyreStrategies");

            migrationBuilder.DropColumn(
                name: "TyreStrategyId",
                table: "TyreStrategies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TyreStrategies",
                table: "TyreStrategies",
                columns: new[] { "StrategyId", "TyreId" });
        }
    }
}
