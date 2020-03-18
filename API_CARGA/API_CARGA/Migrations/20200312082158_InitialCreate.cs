using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RepositoryConfig",
                columns: table => new
                {
                    RepositoryConfigID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OauthToken = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryConfig", x => x.RepositoryConfigID);
                });

            migrationBuilder.CreateTable(
                name: "ShapeConfig",
                columns: table => new
                {
                    ShapeConfigID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    EntityClass = table.Column<string>(nullable: true),
                    Shape = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShapeConfig", x => x.ShapeConfigID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RepositoryConfig");

            migrationBuilder.DropTable(
                name: "ShapeConfig");
        }
    }
}
