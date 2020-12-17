using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Update_DiscoverItem_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDCandidate",
                table: "DiscoverDissambiguation");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "DiscoverDissambiguation");

            migrationBuilder.CreateTable(
                name: "DiscoverDissambiguationCandiate",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DiscoverDissambiguationID = table.Column<Guid>(nullable: false),
                    IDCandidate = table.Column<string>(nullable: false),
                    Score = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoverDissambiguationCandiate", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DiscoverDissambiguationCandiate_DiscoverDissambiguation_Dis~",
                        column: x => x.DiscoverDissambiguationID,
                        principalTable: "DiscoverDissambiguation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscoverDissambiguationCandiate_DiscoverDissambiguationID",
                table: "DiscoverDissambiguationCandiate",
                column: "DiscoverDissambiguationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscoverDissambiguationCandiate");

            migrationBuilder.AddColumn<string>(
                name: "IDCandidate",
                table: "DiscoverDissambiguation",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "DiscoverDissambiguation",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
