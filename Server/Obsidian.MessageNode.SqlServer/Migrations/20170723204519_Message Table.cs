using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Obsidian.MessageNode.SqlServer.Migrations
{
    public partial class MessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DynamicPublicKey = table.Column<byte[]>(nullable: true),
                    DynamicPublicKeyId = table.Column<long>(nullable: false),
                    EncryptedDateUtc = table.Column<DateTime>(nullable: false),
                    ImageCipher = table.Column<byte[]>(nullable: true),
                    MessageType = table.Column<int>(nullable: false),
                    PrivateKeyHint = table.Column<long>(nullable: false),
                    RecipientId = table.Column<string>(nullable: true),
                    SenderId = table.Column<string>(nullable: true),
                    TextCipher = table.Column<byte[]>(nullable: true),
                    UserInfoId = table.Column<string>(nullable: true),
                    XMessageId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_UserInfos_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "UserInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserInfoId",
                table: "Messages",
                column: "UserInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
