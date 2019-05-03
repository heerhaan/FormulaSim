using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class RemoveQualyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.AddColumn<int>(
                name: "Grid",
                table: "DriverResults",
                nullable: true,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grid",
                table: "DriverResults");

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    QualyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DriverRef = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.QualyId);
                    table.ForeignKey(
                        name: "FK_Qualifications_DriverResults_DriverRef",
                        column: x => x.DriverRef,
                        principalTable: "DriverResults",
                        principalColumn: "DriverResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Qualifications_DriverRef",
                table: "Qualifications",
                column: "DriverRef",
                unique: true);
        }
    }
}
