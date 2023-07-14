using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBlueArchiveBot.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGachaRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserGachaRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    TotalGachaCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    ThreeStarCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    PickUpCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGachaRecord", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGachaRecord");
        }
    }
}
