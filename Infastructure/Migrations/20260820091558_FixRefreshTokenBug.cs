using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRefreshTokenBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "RefreshToken",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId1",
                table: "RefreshToken",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_Users_UserId1",
                table: "RefreshToken",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_Users_UserId1",
                table: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_RefreshToken_UserId1",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "RefreshToken");
        }
    }
}
