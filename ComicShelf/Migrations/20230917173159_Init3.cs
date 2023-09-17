using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Flag",
                table: "Country",
                newName: "FlagSVG");

            migrationBuilder.AddColumn<string>(
                name: "FlagPNG",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagPNG",
                table: "Country");

            migrationBuilder.RenameColumn(
                name: "FlagSVG",
                table: "Country",
                newName: "Flag");
        }
    }
}
