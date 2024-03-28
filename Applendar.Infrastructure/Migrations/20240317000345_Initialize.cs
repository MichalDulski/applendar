using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applendar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplendarUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Preferences_NotifyAboutOnlineEvents = table.Column<bool>(type: "boolean", nullable: false),
                    Preferences_NotifyAboutOfflineEvents = table.Column<bool>(type: "boolean", nullable: false),
                    Preferences_NotifyAboutEventsWithPets = table.Column<bool>(type: "boolean", nullable: false),
                    Preferences_NotifyAboutEventsWithCompanions = table.Column<bool>(type: "boolean", nullable: false),
                    LastActivityDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplendarUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    IsCompanionAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    IsPetAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    Location_IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Location_Url = table.Column<string>(type: "text", nullable: true),
                    Location_Name = table.Column<string>(type: "text", nullable: true),
                    Location_City = table.Column<string>(type: "text", nullable: true),
                    Location_ZipCode = table.Column<string>(type: "text", nullable: true),
                    Location_Address = table.Column<string>(type: "text", nullable: true),
                    Location_Country = table.Column<string>(type: "text", nullable: true),
                    MaximumNumberOfParticipants = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OrganizerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_ApplendarUsers_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "ApplendarUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventInvitations",
                columns: table => new
                {
                    ApplendarUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInvitations", x => new { x.ApplendarUserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventInvitations_ApplendarUsers_ApplendarUserId",
                        column: x => x.ApplendarUserId,
                        principalTable: "ApplendarUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventInvitations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitations_EventId",
                table: "EventInvitations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OrganizerId",
                table: "Events",
                column: "OrganizerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInvitations");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ApplendarUsers");
        }
    }
}
