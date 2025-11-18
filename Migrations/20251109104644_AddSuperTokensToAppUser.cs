using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace raptorSlot.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperTokensToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuperTokens",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuperTokens",
                table: "AspNetUsers");
        }
    }
}
