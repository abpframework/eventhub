using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_Url_To_Event : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "AppEvents",
                type: "nvarchar(69)",
                maxLength: 69,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlCode",
                table: "AppEvents",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppEvents_UrlCode",
                table: "AppEvents",
                column: "UrlCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppEvents_UrlCode",
                table: "AppEvents");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "AppEvents");

            migrationBuilder.DropColumn(
                name: "UrlCode",
                table: "AppEvents");
        }
    }
}
