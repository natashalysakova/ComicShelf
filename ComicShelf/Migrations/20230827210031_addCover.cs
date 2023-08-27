using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class addCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Publisher",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "VolumeCovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Cover = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolumeCovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolumeCovers_Volume_Id",
                        column: x => x.Id,
                        principalTable: "Volume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher");

            migrationBuilder.DropTable(
                name: "VolumeCovers");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Publisher",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Publisher_Country_CountryId",
                table: "Publisher",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
