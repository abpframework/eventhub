using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_IsEmailSentToMembers_To_Event : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailSentToMembers",
                table: "AppEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailSentToMembers",
                table: "AppEvents");
        }
    }
}
