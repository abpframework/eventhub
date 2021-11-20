using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.Migrations
{
    public partial class Remove_Payment_From_Main_Database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayPaymentRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayPaymentRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    FailReason = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    State = table.Column<byte>(type: "smallint", nullable: false)
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
    }
}
