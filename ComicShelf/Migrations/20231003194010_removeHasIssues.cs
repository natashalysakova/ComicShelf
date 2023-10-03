using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class removeHasIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasIssues",
                table: "Series");

            migrationBuilder.RenameColumn(
                name: "TotalIssues",
                table: "Series",
                newName: "TotalVolumes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Volume",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalVolumes",
                table: "Series",
                newName: "TotalIssues");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Volume",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HasIssues",
                table: "Series",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
