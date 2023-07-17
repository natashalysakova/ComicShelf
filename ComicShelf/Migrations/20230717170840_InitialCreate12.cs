using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Series",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HasIssues",
                table: "Series",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalIssues",
                table: "Series",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "HasIssues",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "TotalIssues",
                table: "Series");
        }
    }
}
