using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatMicroservice.Migrations
{
    public partial class removedPropsFromConvTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverDelete",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderDelete",
                table: "Conversations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceiverDelete",
                table: "Conversations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SenderDelete",
                table: "Conversations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
