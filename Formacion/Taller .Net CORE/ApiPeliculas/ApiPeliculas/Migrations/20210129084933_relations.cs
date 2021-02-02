using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class relations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Film_Person_DirectorPerson_ID",
                table: "Film");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Film_Film_ID1",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_Film_ID1",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Actor_Film_ID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Film_ID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Film_ID1",
                table: "Person");

            migrationBuilder.RenameColumn(
                name: "DirectorPerson_ID",
                table: "Film",
                newName: "Director_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Film_DirectorPerson_ID",
                table: "Film",
                newName: "IX_Film_Director_ID");

            migrationBuilder.CreateTable(
                name: "Actor",
                columns: table => new
                {
                    Actor_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Person_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actor", x => x.Actor_ID);
                });

            migrationBuilder.CreateTable(
                name: "Director",
                columns: table => new
                {
                    Director_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Person_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Director", x => x.Director_ID);
                });

            migrationBuilder.CreateTable(
                name: "FilmActor",
                columns: table => new
                {
                    Film_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Actor_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmActor", x => new { x.Actor_ID, x.Film_ID });
                    table.ForeignKey(
                        name: "FK_FilmActor_Actor_Actor_ID",
                        column: x => x.Actor_ID,
                        principalTable: "Actor",
                        principalColumn: "Actor_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmActor_Film_Film_ID",
                        column: x => x.Film_ID,
                        principalTable: "Film",
                        principalColumn: "Film_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmActor_Film_ID",
                table: "FilmActor",
                column: "Film_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Film_Director_Director_ID",
                table: "Film",
                column: "Director_ID",
                principalTable: "Director",
                principalColumn: "Director_ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Film_Director_Director_ID",
                table: "Film");

            migrationBuilder.DropTable(
                name: "Director");

            migrationBuilder.DropTable(
                name: "FilmActor");

            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.RenameColumn(
                name: "Director_ID",
                table: "Film",
                newName: "DirectorPerson_ID");

            migrationBuilder.RenameIndex(
                name: "IX_Film_Director_ID",
                table: "Film",
                newName: "IX_Film_DirectorPerson_ID");

            migrationBuilder.AddColumn<Guid>(
                name: "Actor_Film_ID",
                table: "Person",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Person",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "Film_ID",
                table: "Person",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Film_ID1",
                table: "Person",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_Film_ID1",
                table: "Person",
                column: "Film_ID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Film_Person_DirectorPerson_ID",
                table: "Film",
                column: "DirectorPerson_ID",
                principalTable: "Person",
                principalColumn: "Person_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Film_Film_ID1",
                table: "Person",
                column: "Film_ID1",
                principalTable: "Film",
                principalColumn: "Film_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
