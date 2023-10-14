using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applander.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameInvitationsAndEventsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_ApplendarUsers_OrganizerId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_EventInvitation_ApplendarUsers_ApplendarUserId",
                table: "EventInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_EventInvitation_Event_EventId",
                table: "EventInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventInvitation",
                table: "EventInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                table: "Event");

            migrationBuilder.RenameTable(
                name: "EventInvitation",
                newName: "EventInvitations");

            migrationBuilder.RenameTable(
                name: "Event",
                newName: "Events");

            migrationBuilder.RenameIndex(
                name: "IX_EventInvitation_EventId",
                table: "EventInvitations",
                newName: "IX_EventInvitations_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Event_OrganizerId",
                table: "Events",
                newName: "IX_Events_OrganizerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventInvitations",
                table: "EventInvitations",
                columns: new[] { "ApplendarUserId", "EventId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInvitations_ApplendarUsers_ApplendarUserId",
                table: "EventInvitations",
                column: "ApplendarUserId",
                principalTable: "ApplendarUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInvitations_Events_EventId",
                table: "EventInvitations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ApplendarUsers_OrganizerId",
                table: "Events",
                column: "OrganizerId",
                principalTable: "ApplendarUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventInvitations_ApplendarUsers_ApplendarUserId",
                table: "EventInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_EventInvitations_Events_EventId",
                table: "EventInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_ApplendarUsers_OrganizerId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventInvitations",
                table: "EventInvitations");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Event");

            migrationBuilder.RenameTable(
                name: "EventInvitations",
                newName: "EventInvitation");

            migrationBuilder.RenameIndex(
                name: "IX_Events_OrganizerId",
                table: "Event",
                newName: "IX_Event_OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventInvitations_EventId",
                table: "EventInvitation",
                newName: "IX_EventInvitation_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventInvitation",
                table: "EventInvitation",
                columns: new[] { "ApplendarUserId", "EventId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Event_ApplendarUsers_OrganizerId",
                table: "Event",
                column: "OrganizerId",
                principalTable: "ApplendarUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventInvitation_ApplendarUsers_ApplendarUserId",
                table: "EventInvitation",
                column: "ApplendarUserId",
                principalTable: "ApplendarUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventInvitation_Event_EventId",
                table: "EventInvitation",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");
        }
    }
}
