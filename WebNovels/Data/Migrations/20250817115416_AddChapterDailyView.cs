using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNovels.Migrations
{
    /// <inheritdoc />
    public partial class AddChapterDailyView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChapterDailyViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NovelId = table.Column<int>(type: "int", nullable: false),
                    ChapterId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterDailyViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterDailyViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChapterDailyViews_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChapterDailyViews_Novels_NovelId",
                        column: x => x.NovelId,
                        principalTable: "Novels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterDailyViews_ChapterId",
                table: "ChapterDailyViews",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterDailyViews_NovelId",
                table: "ChapterDailyViews",
                column: "NovelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterDailyViews_UserId_ChapterId_Day",
                table: "ChapterDailyViews",
                columns: new[] { "UserId", "ChapterId", "Day" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterDailyViews");
        }
    }
}
