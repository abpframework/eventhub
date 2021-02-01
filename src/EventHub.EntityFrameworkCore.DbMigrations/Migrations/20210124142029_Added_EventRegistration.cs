using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_EventRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppEventRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEventRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEventRegistrations_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppEventRegistrations_AppEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "AppEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOrganizations_OwnerUserId",
                table: "AppOrganizations",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppEventRegistrations_EventId_UserId",
                table: "AppEventRegistrations",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppEventRegistrations_UserId",
                table: "AppEventRegistrations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEvents_AppOrganizations_OrganizationId",
                table: "AppEvents",
                column: "OrganizationId",
                principalTable: "AppOrganizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppOrganizations_AbpUsers_OwnerUserId",
                table: "AppOrganizations",
                column: "OwnerUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEvents_AppOrganizations_OrganizationId",
                table: "AppEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AppOrganizations_AbpUsers_OwnerUserId",
                table: "AppOrganizations");

            migrationBuilder.DropTable(
                name: "AppEventRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_AppOrganizations_OwnerUserId",
                table: "AppOrganizations");
        }
    }
}
