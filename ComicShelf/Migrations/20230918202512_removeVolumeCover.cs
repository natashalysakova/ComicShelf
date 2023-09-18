using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class removeVolumeCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolumeCovers");

            migrationBuilder.DropColumn(
                name: "CoverId",
                table: "Volume");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoverId",
                table: "Volume",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VolumeCovers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Cover = table.Column<byte[]>(type: "longblob", nullable: false),
                    Extention = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
