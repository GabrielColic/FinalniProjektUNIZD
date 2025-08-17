using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNovels.Migrations
{
    /// <inheritdoc />
    public partial class AddReadCountToNovel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReadCount",
                table: "Novels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Novels_ReadCount",
                table: "Novels",
                column: "ReadCount");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Novel_ReadCount_NonNegative",
                table: "Novels",
                sql: "[ReadCount] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Novels_ReadCount",
                table: "Novels");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Novel_ReadCount_NonNegative",
                table: "Novels");

            migrationBuilder.DropColumn(
                name: "ReadCount",
                table: "Novels");
        }
    }
}
