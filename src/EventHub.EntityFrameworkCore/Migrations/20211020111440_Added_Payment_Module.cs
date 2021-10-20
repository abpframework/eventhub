using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Migrations
{
    public partial class Added_Payment_Module : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayPaymentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<byte>(type: "smallint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FailReason = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayPaymentRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayPaymentRequests_CustomerId",
                table: "PayPaymentRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PayPaymentRequests_State",
                table: "PayPaymentRequests",
                column: "State");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayPaymentRequests");
        }
    }
}
