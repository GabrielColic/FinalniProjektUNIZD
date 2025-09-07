using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNovels.Migrations
{
    /// <inheritdoc />
    public partial class Cascade_Novel_Chapters_Bookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Novels_NovelId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Novels_AspNetUsers_AuthorId",
                table: "Novels");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Novels_NovelId",
                table: "Bookmarks",
                column: "NovelId",
                principalTable: "Novels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Novels_AspNetUsers_AuthorId",
                table: "Novels",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Novels_NovelId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Novels_AspNetUsers_AuthorId",
                table: "Novels");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Novels_NovelId",
                table: "Bookmarks",
                column: "NovelId",
                principalTable: "Novels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Novels_AspNetUsers_AuthorId",
                table: "Novels",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
