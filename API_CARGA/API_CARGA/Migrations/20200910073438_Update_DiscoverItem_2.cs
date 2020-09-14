using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    public partial class Update_DiscoverItem_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DissambiguationProcessed",
                table: "DiscoverItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "JobCreatedDate",
                table: "DiscoverItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobID",
                table: "DiscoverItem",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Publish",
                table: "DiscoverItem",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DiscoverDissambiguation",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    DiscoverItemID = table.Column<Guid>(nullable: false),
                    IDOrigin = table.Column<string>(nullable: false),
                    IDCandidate = table.Column<string>(nullable: false),
                    Score = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoverDissambiguation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DiscoverDissambiguation_DiscoverItem_DiscoverItemID",
                        column: x => x.DiscoverItemID,
                        principalTable: "DiscoverItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscoverDissambiguation_DiscoverItemID",
                table: "DiscoverDissambiguation",
                column: "DiscoverItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscoverDissambiguation");

            migrationBuilder.DropColumn(
                name: "DissambiguationProcessed",
                table: "DiscoverItem");

            migrationBuilder.DropColumn(
                name: "JobCreatedDate",
                table: "DiscoverItem");

            migrationBuilder.DropColumn(
                name: "JobID",
                table: "DiscoverItem");

            migrationBuilder.DropColumn(
                name: "Publish",
                table: "DiscoverItem");
        }
    }
}
