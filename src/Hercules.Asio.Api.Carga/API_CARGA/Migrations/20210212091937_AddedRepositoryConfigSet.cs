using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddedRepositoryConfigSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepositoryConfigSet",
                columns: table => new
                {
                    RepositoryConfigSetID = table.Column<Guid>(nullable: false),
                    Set = table.Column<string>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    RepositoryID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryConfigSet", x => x.RepositoryConfigSetID);
                    table.ForeignKey(
                        name: "FK_RepositoryConfigSet_RepositoryConfig_RepositoryID",
                        column: x => x.RepositoryID,
                        principalTable: "RepositoryConfig",
                        principalColumn: "RepositoryConfigID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepositoryConfigSet_RepositoryID",
                table: "RepositoryConfigSet",
                column: "RepositoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepositoryConfigSet");
        }
    }
}
