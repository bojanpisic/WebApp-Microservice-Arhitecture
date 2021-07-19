using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatMicroservice.Migrations
{
    public partial class conversationTableChanged2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceiverDelete",
                table: "Conversations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SenderDelete",
                table: "Conversations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverDelete",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderDelete",
                table: "Conversations");
        }
    }
}
