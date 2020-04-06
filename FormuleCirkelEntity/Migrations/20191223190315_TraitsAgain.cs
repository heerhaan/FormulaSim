﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class TraitsAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_Abbreviation",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "Traits",
                table: "Tracks",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Teams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Traits",
                table: "SeasonTeams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Traits",
                table: "SeasonDrivers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Traits",
                columns: table => new
                {
                    TraitId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TraitGroup = table.Column<int>(nullable: false),
                    TraitDescription = table.Column<string>(nullable: true),
                    QualyPace = table.Column<int>(nullable: true),
                    DriverRacePace = table.Column<int>(nullable: true),
                    ChassisRacePace = table.Column<int>(nullable: true),
                    ChassisReliability = table.Column<int>(nullable: true),
                    DriverReliability = table.Column<int>(nullable: true),
                    MaximumRNG = table.Column<int>(nullable: true),
                    MinimumRNG = table.Column<int>(nullable: true),
                    ChassisMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DriverMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EngineMultiplier = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traits", x => x.TraitId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Traits");

            migrationBuilder.DropColumn(
                name: "Traits",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Traits",
                table: "SeasonTeams");

            migrationBuilder.DropColumn(
                name: "Traits",
                table: "SeasonDrivers");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Teams",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}