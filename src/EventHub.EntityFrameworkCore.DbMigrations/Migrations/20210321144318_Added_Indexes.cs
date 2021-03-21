using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_Indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EhEvents_IsEmailSentToMembers",
                table: "EhEvents",
                column: "IsEmailSentToMembers");

            migrationBuilder.CreateIndex(
                name: "IX_EhEvents_IsRemindingEmailSent_StartTime",
                table: "EhEvents",
                columns: new[] { "IsRemindingEmailSent", "StartTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EhEvents_IsEmailSentToMembers",
                table: "EhEvents");

            migrationBuilder.DropIndex(
                name: "IX_EhEvents_IsRemindingEmailSent_StartTime",
                table: "EhEvents");
        }
    }
}
