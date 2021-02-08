using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Director",
                columns: table => new
                {
                    Person_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Film_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Director", x => x.Person_ID);
                });

            migrationBuilder.CreateTable(
                name: "Film",
                columns: table => new
                {
                    Film_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Released = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinuteRunTime = table.Column<int>(type: "int", nullable: false),
                    DirectorPerson_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Film", x => x.Film_ID);
                    table.ForeignKey(
                        name: "FK_Film_Director_DirectorPerson_ID",
                        column: x => x.DirectorPerson_ID,
                        principalTable: "Director",
                        principalColumn: "Person_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actor",
                columns: table => new
                {
                    Person_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Film_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Film_ID1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actor", x => x.Person_ID);
                    table.ForeignKey(
                        name: "FK_Actor_Film_Film_ID1",
                        column: x => x.Film_ID1,
                        principalTable: "Film",
                        principalColumn: "Film_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Rating_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Film_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Film_ID1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Rating_ID);
                    table.ForeignKey(
                        name: "FK_Rating_Film_Film_ID1",
                        column: x => x.Film_ID1,
                        principalTable: "Film",
                        principalColumn: "Film_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actor_Film_ID1",
                table: "Actor",
                column: "Film_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_Film_DirectorPerson_ID",
                table: "Film",
                column: "DirectorPerson_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_Film_ID1",
                table: "Rating",
                column: "Film_ID1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "Film");

            migrationBuilder.DropTable(
                name: "Director");
        }
    }
}
