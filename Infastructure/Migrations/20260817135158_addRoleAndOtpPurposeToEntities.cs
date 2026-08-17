using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRoleAndOtpPurposeToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerification",
                table: "OtpVerification");

            migrationBuilder.DropIndex(
                name: "IX_OtpVerification_PhoneNumber",
                table: "OtpVerification");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "OtpVerification",
                newName: "OtpVerifications");

            migrationBuilder.RenameIndex(
                name: "IX_User_PhoneNumber",
                table: "Users",
                newName: "IX_Users_PhoneNumber");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "CodeHash",
                table: "OtpVerifications",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "OtpVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications",
                column: "OtpVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_PhoneNumber_CreatedAt",
                table: "OtpVerifications",
                columns: new[] { "PhoneNumber", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications");

            migrationBuilder.DropIndex(
                name: "IX_OtpVerifications_PhoneNumber_CreatedAt",
                table: "OtpVerifications");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "OtpVerifications");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "OtpVerifications",
                newName: "OtpVerification");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PhoneNumber",
                table: "User",
                newName: "IX_User_PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Customer",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CodeHash",
                table: "OtpVerification",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerification",
                table: "OtpVerification",
                column: "OtpVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerification_PhoneNumber",
                table: "OtpVerification",
                column: "PhoneNumber");
        }
    }
}
