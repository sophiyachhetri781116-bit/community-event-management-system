using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityEventManagement.Migrations
{
    /// <inheritdoc />
    public partial class LinkParticipantToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Participants");
        }
    }
}
