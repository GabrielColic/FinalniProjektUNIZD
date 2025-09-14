using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNovels.Migrations
{
    /// <inheritdoc />
    public partial class CommentThreading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ChapterId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "Depth",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RootCommentId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ChapterId_ParentCommentId_CreatedAt",
                table: "Comments",
                columns: new[] { "ChapterId", "ParentCommentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ChapterId_RootCommentId_CreatedAt",
                table: "Comments",
                columns: new[] { "ChapterId", "RootCommentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Comment_Depth_NonNegative",
                table: "Comments",
                sql: "[Depth] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ChapterId_ParentCommentId_CreatedAt",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ChapterId_RootCommentId_CreatedAt",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Comment_Depth_NonNegative",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "RootCommentId",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ChapterId",
                table: "Comments",
                column: "ChapterId");
        }
    }
}
