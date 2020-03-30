using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API_CARGA.Migrations
{
    public partial class adSyncConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncConfig",
                columns: table => new
                {
                    SyncConfigID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    StartHour = table.Column<string>(nullable: true),
                    UpdateFrequency = table.Column<int>(nullable: false),
                    RepositoryIdentifier = table.Column<Guid>(nullable: false),
                    RepositorySetIdentifiers = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncConfig", x => x.SyncConfigID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncConfig");
        }
    }
}
