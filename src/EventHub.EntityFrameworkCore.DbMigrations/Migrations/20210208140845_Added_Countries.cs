using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_Countries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "AppEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppCountries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCountries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEvents_CountryId",
                table: "AppEvents",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCountries_Name",
                table: "AppCountries",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_AppEvents_AppCountries_CountryId",
                table: "AppEvents",
                column: "CountryId",
                principalTable: "AppCountries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEvents_AppCountries_CountryId",
                table: "AppEvents");

            migrationBuilder.DropTable(
                name: "AppCountries");

            migrationBuilder.DropIndex(
                name: "IX_AppEvents_CountryId",
                table: "AppEvents");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "AppEvents");
        }
    }
}
