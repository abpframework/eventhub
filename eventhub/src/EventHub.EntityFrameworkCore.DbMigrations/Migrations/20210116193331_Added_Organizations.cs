using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_Organizations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GitHubUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacebookUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediumUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrganizations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizations_DisplayName",
                table: "AppOrganizations",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizations_Name",
                table: "AppOrganizations",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppOrganizations");
        }
    }
}
