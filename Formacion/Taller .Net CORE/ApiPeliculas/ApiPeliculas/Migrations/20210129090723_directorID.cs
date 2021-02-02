using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class directorID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Film_Director_Director_ID",
                table: "Film");

            migrationBuilder.AlterColumn<Guid>(
                name: "Director_ID",
                table: "Film",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Film_Director_Director_ID",
                table: "Film",
                column: "Director_ID",
                principalTable: "Director",
                principalColumn: "Director_ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Film_Director_Director_ID",
                table: "Film");

            migrationBuilder.AlterColumn<Guid>(
                name: "Director_ID",
                table: "Film",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Film_Director_Director_ID",
                table: "Film",
                column: "Director_ID",
                principalTable: "Director",
                principalColumn: "Director_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
