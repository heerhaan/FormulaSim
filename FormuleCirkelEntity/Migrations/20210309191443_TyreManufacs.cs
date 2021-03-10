using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TyreManufacs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rubbers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Colour = table.Column<string>(nullable: true),
                    Accent = table.Column<string>(nullable: true),
                    PaceMod = table.Column<string>(nullable: true),
                    MaxWearMod = table.Column<string>(nullable: true),
                    MinWearMod = table.Column<string>(nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubbers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rubbers_Archived",
                table: "Rubbers",
                column: "Archived");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rubbers");
        }
    }
}
