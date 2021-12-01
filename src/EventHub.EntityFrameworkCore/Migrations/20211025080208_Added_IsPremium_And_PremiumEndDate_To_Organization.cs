using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Migrations
{
    public partial class Added_IsPremium_And_PremiumEndDate_To_Organization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPremium",
                table: "EhOrganizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PremiumEndDate",
                table: "EhOrganizations",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPremium",
                table: "EhOrganizations");

            migrationBuilder.DropColumn(
                name: "PremiumEndDate",
                table: "EhOrganizations");
        }
    }
}
