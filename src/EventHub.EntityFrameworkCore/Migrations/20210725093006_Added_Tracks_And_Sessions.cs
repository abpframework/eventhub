using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventHub.Migrations
{
    public partial class Added_Tracks_And_Sessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "EhEvents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EhEventTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EhEventTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EhEventTracks_EhEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "EhEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EhEventSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Language = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EhEventSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EhEventSessions_EhEventTracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "EhEventTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EhEventSpeakers",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EhEventSpeakers", x => new { x.SessionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_EhEventSpeakers_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EhEventSpeakers_EhEventSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "EhEventSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EhEventSessions_TrackId",
                table: "EhEventSessions",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_EhEventSpeakers_UserId_SessionId",
                table: "EhEventSpeakers",
                columns: new[] { "UserId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_EhEventTracks_EventId",
                table: "EhEventTracks",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EhEventSpeakers");

            migrationBuilder.DropTable(
                name: "EhEventSessions");

            migrationBuilder.DropTable(
                name: "EhEventTracks");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "EhEvents");
        }
    }
}
