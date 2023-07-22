using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Roles",
                table: "Author",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Author");
        }
    }
}
