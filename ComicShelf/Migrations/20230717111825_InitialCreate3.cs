using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Volume_VolumeId",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher");

            migrationBuilder.DropForeignKey(
                name: "FK_Volume_Series_SeriesId",
                table: "Volume");

            migrationBuilder.DropTable(
                name: "PublisherSeries");

            migrationBuilder.DropIndex(
                name: "IX_Volume_SeriesId",
                table: "Volume");

            migrationBuilder.DropIndex(
                name: "IX_Issue_VolumeId",
                table: "Issue");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                table: "Volume",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublisherId",
                table: "Series",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Publisher",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VolumeId",
                table: "Issue",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Series_PublisherId",
                table: "Series",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Publisher_PublisherId",
                table: "Series",
                column: "PublisherId",
                principalTable: "Publisher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher");

            migrationBuilder.DropForeignKey(
                name: "FK_Series_Publisher_PublisherId",
                table: "Series");

            migrationBuilder.DropIndex(
                name: "IX_Series_PublisherId",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Series");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                table: "Volume",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Publisher",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "VolumeId",
                table: "Issue",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
                name: "IX_Volume_SeriesId",
                table: "Volume",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Issue_VolumeId",
                table: "Issue",
                column: "VolumeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublisherSeries_SeriesId",
                table: "PublisherSeries",
                column: "SeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Volume_VolumeId",
                table: "Issue",
                column: "VolumeId",
                principalTable: "Volume",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Volume_Series_SeriesId",
                table: "Volume",
                column: "SeriesId",
                principalTable: "Series",
                principalColumn: "Id");
        }
    }
}
