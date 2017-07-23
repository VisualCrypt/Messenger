using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Obsidian.MessageNode.SqlServer.Migrations
{
    public partial class IdentityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Identities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ContactState = table.Column<int>(nullable: false),
                    LastSeenUTC = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PublicIdentityKey = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Identities");
        }
    }
}
