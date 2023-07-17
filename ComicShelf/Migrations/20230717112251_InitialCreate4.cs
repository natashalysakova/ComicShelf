using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComicShelf.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Issue_VolumeId",
                table: "Issue",
                column: "VolumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Volume_VolumeId",
                table: "Issue",
                column: "VolumeId",
                principalTable: "Volume",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Volume_VolumeId",
                table: "Issue");

            migrationBuilder.DropIndex(
                name: "IX_Issue_VolumeId",
                table: "Issue");
        }
    }
}
