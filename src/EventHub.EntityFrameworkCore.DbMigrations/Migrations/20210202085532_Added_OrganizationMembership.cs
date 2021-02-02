using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_OrganizationMembership : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppOrganizationMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOrganizationMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOrganizationMemberships_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppOrganizationMemberships_AppOrganizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "AppOrganizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizationMemberships_OrganizationId_UserId",
                table: "AppOrganizationMemberships",
                columns: new[] { "OrganizationId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizationMemberships_UserId",
                table: "AppOrganizationMemberships",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppOrganizationMemberships");
        }
    }
}
