using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNovels.Migrations
{
    /// <inheritdoc />
    public partial class ChapterDailyViews_UserId_Nullable_SetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterDailyViews_AspNetUsers_UserId",
                table: "ChapterDailyViews");

            migrationBuilder.DropIndex(
                name: "IX_ChapterDailyViews_UserId_ChapterId_Day",
                table: "ChapterDailyViews");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ChapterDailyViews",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterDailyViews_UserId_ChapterId_Day",
                table: "ChapterDailyViews",
                columns: new[] { "UserId", "ChapterId", "Day" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterDailyViews_AspNetUsers_UserId",
                table: "ChapterDailyViews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterDailyViews_AspNetUsers_UserId",
                table: "ChapterDailyViews");

            migrationBuilder.DropIndex(
                name: "IX_ChapterDailyViews_UserId_ChapterId_Day",
                table: "ChapterDailyViews");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ChapterDailyViews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChapterDailyViews_UserId_ChapterId_Day",
                table: "ChapterDailyViews",
                columns: new[] { "UserId", "ChapterId", "Day" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterDailyViews_AspNetUsers_UserId",
                table: "ChapterDailyViews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
