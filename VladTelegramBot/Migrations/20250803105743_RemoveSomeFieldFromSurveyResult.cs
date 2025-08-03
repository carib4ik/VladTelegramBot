using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VladTelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSomeFieldFromSurveyResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SurveyResults_TelegramId",
                table: "SurveyResults");

            migrationBuilder.DropColumn(
                name: "TelegramId",
                table: "SurveyResults");

            migrationBuilder.AlterColumn<string>(
                name: "TelegramName",
                table: "SurveyResults",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResults_ChatId",
                table: "SurveyResults",
                column: "ChatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SurveyResults_ChatId",
                table: "SurveyResults");

            migrationBuilder.AlterColumn<string>(
                name: "TelegramName",
                table: "SurveyResults",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TelegramId",
                table: "SurveyResults",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResults_TelegramId",
                table: "SurveyResults",
                column: "TelegramId",
                unique: true);
        }
    }
}
