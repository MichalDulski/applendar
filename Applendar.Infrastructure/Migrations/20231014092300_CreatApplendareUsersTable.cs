using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Applander.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatApplendareUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizerId",
                table: "Event",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ApplendarUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplendarUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizerId",
                table: "Event",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_ApplendarUsers_OrganizerId",
                table: "Event",
                column: "OrganizerId",
                principalTable: "ApplendarUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_ApplendarUsers_OrganizerId",
                table: "Event");

            migrationBuilder.DropTable(
                name: "ApplendarUsers");

            migrationBuilder.DropIndex(
                name: "IX_Event_OrganizerId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "OrganizerId",
                table: "Event");
        }
    }
}
