using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatMicroservice.Migrations
{
    public partial class conversationTableChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendFirstname",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "FriendLastName",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverFirstname",
                table: "Conversations",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverLastName",
                table: "Conversations",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "SenderFirstname",
                table: "Conversations",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "SenderLastName",
                table: "Conversations",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverFirstname",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ReceiverLastName",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderFirstname",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderLastName",
                table: "Conversations");

            migrationBuilder.AddColumn<string>(
                name: "FriendFirstname",
                table: "Conversations",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FriendLastName",
                table: "Conversations",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");
        }
    }
}
