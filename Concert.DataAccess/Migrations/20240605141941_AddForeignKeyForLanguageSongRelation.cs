using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concert.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyForLanguageSongRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 1,
                column: "LanguageId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 2,
                column: "LanguageId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 3,
                column: "LanguageId",
                value: 3);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_LanguageId",
                table: "Songs",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Languages_LanguageId",
                table: "Songs",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Languages_LanguageId",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_LanguageId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Songs");
        }
    }
}
