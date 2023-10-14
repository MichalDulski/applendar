using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applander.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NotifyAboutEventsWithCompanions",
                table: "ApplendarUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NotifyAboutEventsWithPets",
                table: "ApplendarUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NotifyAboutOfflineEvents",
                table: "ApplendarUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Preferences_NotifyAboutOnlineEvents",
                table: "ApplendarUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preferences_NotifyAboutEventsWithCompanions",
                table: "ApplendarUsers");

            migrationBuilder.DropColumn(
                name: "Preferences_NotifyAboutEventsWithPets",
                table: "ApplendarUsers");

            migrationBuilder.DropColumn(
                name: "Preferences_NotifyAboutOfflineEvents",
                table: "ApplendarUsers");

            migrationBuilder.DropColumn(
                name: "Preferences_NotifyAboutOnlineEvents",
                table: "ApplendarUsers");
        }
    }
}
