using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace raptorSlot.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Osoby",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    imie = table.Column<string>(type: "TEXT", nullable: false),
                    nazwisko = table.Column<string>(type: "TEXT", nullable: false),
                    waga = table.Column<int>(type: "INTEGER", nullable: false),
                    wzrostCM = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Osoby", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Osoby");
        }
    }
}
