using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class Person : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Film_Director_DirectorPerson_ID",
                table: "Film");

            migrationBuilder.DropTable(
                name: "Actor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Director",
                table: "Director");

            migrationBuilder.RenameTable(
                name: "Director",
                newName: "Person");

            migrationBuilder.AlterColumn<Guid>(
                name: "Film_ID",
                table: "Person",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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
                name: "Film_ID1",
                table: "Person",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Person",
                table: "Person",
                column: "Person_ID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Film_Person_DirectorPerson_ID",
                table: "Film");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Film_Film_ID1",
                table: "Person");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
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
                name: "Film_ID1",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "Director");

            migrationBuilder.AlterColumn<Guid>(
                name: "Film_ID",
                table: "Director",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Director",
                table: "Director",
                column: "Person_ID");

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

            migrationBuilder.CreateIndex(
                name: "IX_Actor_Film_ID1",
                table: "Actor",
                column: "Film_ID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Film_Director_DirectorPerson_ID",
                table: "Film",
                column: "DirectorPerson_ID",
                principalTable: "Director",
                principalColumn: "Person_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
