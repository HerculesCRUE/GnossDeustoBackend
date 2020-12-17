using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Discard_Dissambiguation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscardDissambiguation",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DiscoverItemID = table.Column<Guid>(nullable: false),
                    IDOrigin = table.Column<string>(nullable: false),
                    DiscardCandidates = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscardDissambiguation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DiscardDissambiguation_DiscoverItem_DiscoverItemID",
                        column: x => x.DiscoverItemID,
                        principalTable: "DiscoverItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscardDissambiguation_DiscoverItemID",
                table: "DiscardDissambiguation",
                column: "DiscoverItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscardDissambiguation");
        }
    }
}
