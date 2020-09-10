using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    public partial class Update_DiscoverItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscoverRdf",
                table: "DiscoverItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "DiscoverItem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscoverRdf",
                table: "DiscoverItem");

            migrationBuilder.DropColumn(
                name: "Error",
                table: "DiscoverItem");
        }
    }
}
