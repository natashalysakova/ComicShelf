using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Series_Publisher_PublisherId",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Series_PublisherId",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Series");

            migrationBuilder.AlterColumn<bool>(
                name: "Ongoing",
                table: "Series",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Series",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PublisherSeries",
                columns: table => new
                {
                    PublishersId = table.Column<int>(type: "int", nullable: false),
                    SeriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublisherSeries", x => new { x.PublishersId, x.SeriesId });
                    table.ForeignKey(
                        name: "FK_PublisherSeries_Publisher_PublishersId",
                        column: x => x.PublishersId,
                        principalTable: "Publisher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublisherSeries_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublisherSeries_SeriesId",
                table: "PublisherSeries",
                column: "SeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublisherSeries");

            migrationBuilder.AlterColumn<bool>(
                name: "Ongoing",
                table: "Series",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Series",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "PublisherId",
                table: "Series",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Series_PublisherId",
                table: "Series",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Publisher_PublisherId",
                table: "Series",
                column: "PublisherId",
                principalTable: "Publisher",
                principalColumn: "Id");
        }
    }
}
