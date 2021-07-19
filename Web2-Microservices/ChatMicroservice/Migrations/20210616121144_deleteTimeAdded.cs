using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatMicroservice.Migrations
{
    public partial class deleteTimeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReceiverDeleteTime",
                table: "Conversations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SenderDeleteTime",
                table: "Conversations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverDeleteTime",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderDeleteTime",
                table: "Conversations");
        }
    }
}
