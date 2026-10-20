using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSupportChatEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "Customer",
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupportConversation",
                columns: table => new
                {
                    SupportConversationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAdminId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportConversation", x => x.SupportConversationId);
                    table.ForeignKey(
                        name: "FK_SupportConversation_Users_AssignedAdminId",
                        column: x => x.AssignedAdminId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportConversation_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportMessage",
                columns: table => new
                {
                    SupportMessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupportConversationId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportMessage", x => x.SupportMessageId);
                    table.ForeignKey(
                        name: "FK_SupportMessage_SupportConversation_SupportConversationId",
                        column: x => x.SupportConversationId,
                        principalTable: "SupportConversation",
                        principalColumn: "SupportConversationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportMessage_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportConversation_AssignedAdminId",
                table: "SupportConversation",
                column: "AssignedAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportConversation_Status_CreatedAt",
                table: "SupportConversation",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportConversation_UserId",
                table: "SupportConversation",
                column: "UserId",
                unique: true,
                filter: "\"Status\" IN ('Open', 'InProgress')");

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessage_SenderId",
                table: "SupportMessage",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessage_SupportConversationId_IsRead",
                table: "SupportMessage",
                columns: new[] { "SupportConversationId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessage_SupportConversationId_SentAt_SupportMessageId",
                table: "SupportMessage",
                columns: new[] { "SupportConversationId", "SentAt", "SupportMessageId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportMessage");

            migrationBuilder.DropTable(
                name: "SupportConversation");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Customer");
        }
    }
}
