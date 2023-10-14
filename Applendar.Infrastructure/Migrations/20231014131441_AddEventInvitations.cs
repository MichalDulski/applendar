using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applander.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventInvitation",
                columns: table => new
                {
                    ApplendarUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInvitation", x => new { x.ApplendarUserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventInvitation_ApplendarUsers_ApplendarUserId",
                        column: x => x.ApplendarUserId,
                        principalTable: "ApplendarUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventInvitation_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitation_EventId",
                table: "EventInvitation",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInvitation");
        }
    }
}
