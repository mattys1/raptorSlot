using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace raptorSlot.Migrations
{
    /// <inheritdoc />
    public partial class RenameAvatarUri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarUri",
                table: "AspNetUsers",
                newName: "AvatarPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvatarPath",
                table: "AspNetUsers",
                newName: "AvatarUri");
        }
    }
}
