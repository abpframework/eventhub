using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Moved_IsTimingChangeEmailSent_To_Event : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTimingChangeEmailSent",
                table: "EhEventRegistrations");

            migrationBuilder.AddColumn<bool>(
                name: "IsTimingChangeEmailSent",
                table: "EhEvents",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTimingChangeEmailSent",
                table: "EhEvents");

            migrationBuilder.AddColumn<bool>(
                name: "IsTimingChangeEmailSent",
                table: "EhEventRegistrations",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
