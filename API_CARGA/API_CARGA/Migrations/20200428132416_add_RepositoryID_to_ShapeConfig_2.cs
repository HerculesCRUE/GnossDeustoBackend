using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    public partial class add_RepositoryID_to_ShapeConfig_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShapeConfig_RepositoryID",
                table: "ShapeConfig",
                column: "RepositoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShapeConfig_RepositoryConfig_RepositoryID",
                table: "ShapeConfig",
                column: "RepositoryID",
                principalTable: "RepositoryConfig",
                principalColumn: "RepositoryConfigID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShapeConfig_RepositoryConfig_RepositoryID",
                table: "ShapeConfig");

            migrationBuilder.DropIndex(
                name: "IX_ShapeConfig_RepositoryID",
                table: "ShapeConfig");
        }
    }
}
