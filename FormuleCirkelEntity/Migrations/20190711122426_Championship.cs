﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormuleCirkelEntity.Migrations
{
    public partial class Championship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChampionshipId",
                table: "Seasons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Championships",
                columns: table => new
                {
                    ChampionshipId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChampionshipName = table.Column<string>(nullable: true),
                    ActiveChampionship = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Championships", x => x.ChampionshipId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_ChampionshipId",
                table: "Seasons",
                column: "ChampionshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_Championships_ChampionshipId",
                table: "Seasons",
                column: "ChampionshipId",
                principalTable: "Championships",
                principalColumn: "ChampionshipId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_Championships_ChampionshipId",
                table: "Seasons");

            migrationBuilder.DropTable(
                name: "Championships");

            migrationBuilder.DropIndex(
                name: "IX_Seasons_ChampionshipId",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "ChampionshipId",
                table: "Seasons");
        }
    }
}