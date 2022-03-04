using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Migrations
{
    public partial class Updated_Organization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "EhOrganizations");

            migrationBuilder.RenameColumn(
                name: "PremiumEndDate",
                table: "EhOrganizations",
                newName: "PaidEnrollmentEndDate");

            migrationBuilder.RenameColumn(
                name: "IsSendPremiumReminderEmail",
                table: "EhOrganizations",
                newName: "IsSendPaidEnrollmentReminderEmail");

            migrationBuilder.AddColumn<byte>(
                name: "PlanType",
                table: "EhOrganizations",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanType",
                table: "EhOrganizations");

            migrationBuilder.RenameColumn(
                name: "PaidEnrollmentEndDate",
                table: "EhOrganizations",
                newName: "PremiumEndDate");

            migrationBuilder.RenameColumn(
                name: "IsSendPaidEnrollmentReminderEmail",
                table: "EhOrganizations",
                newName: "IsSendPremiumReminderEmail");

            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "EhOrganizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
