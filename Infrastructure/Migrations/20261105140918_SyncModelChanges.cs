using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportConversation_Users_AssignedAdminId",
                table: "SupportConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportConversation_Users_UserId",
                table: "SupportConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessage_SupportConversation_SupportConversationId",
                table: "SupportMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessage_Users_SenderId",
                table: "SupportMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportMessage",
                table: "SupportMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportConversation",
                table: "SupportConversation");

            migrationBuilder.RenameTable(
                name: "SupportMessage",
                newName: "SupportMessages");

            migrationBuilder.RenameTable(
                name: "SupportConversation",
                newName: "SupportConversations");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessage_SupportConversationId_SentAt_SupportMessageId",
                table: "SupportMessages",
                newName: "IX_SupportMessages_SupportConversationId_SentAt_SupportMessage~");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessage_SupportConversationId_IsRead",
                table: "SupportMessages",
                newName: "IX_SupportMessages_SupportConversationId_IsRead");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessage_SenderId",
                table: "SupportMessages",
                newName: "IX_SupportMessages_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportConversation_UserId",
                table: "SupportConversations",
                newName: "IX_SupportConversations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportConversation_Status_CreatedAt",
                table: "SupportConversations",
                newName: "IX_SupportConversations_Status_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_SupportConversation_AssignedAdminId",
                table: "SupportConversations",
                newName: "IX_SupportConversations_AssignedAdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportMessages",
                table: "SupportMessages",
                column: "SupportMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportConversations",
                table: "SupportConversations",
                column: "SupportConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportConversations_Users_AssignedAdminId",
                table: "SupportConversations",
                column: "AssignedAdminId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportConversations_Users_UserId",
                table: "SupportConversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessages_SupportConversations_SupportConversationId",
                table: "SupportMessages",
                column: "SupportConversationId",
                principalTable: "SupportConversations",
                principalColumn: "SupportConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessages_Users_SenderId",
                table: "SupportMessages",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportConversations_Users_AssignedAdminId",
                table: "SupportConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportConversations_Users_UserId",
                table: "SupportConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessages_SupportConversations_SupportConversationId",
                table: "SupportMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessages_Users_SenderId",
                table: "SupportMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportMessages",
                table: "SupportMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupportConversations",
                table: "SupportConversations");

            migrationBuilder.RenameTable(
                name: "SupportMessages",
                newName: "SupportMessage");

            migrationBuilder.RenameTable(
                name: "SupportConversations",
                newName: "SupportConversation");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_SupportConversationId_SentAt_SupportMessage~",
                table: "SupportMessage",
                newName: "IX_SupportMessage_SupportConversationId_SentAt_SupportMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_SupportConversationId_IsRead",
                table: "SupportMessage",
                newName: "IX_SupportMessage_SupportConversationId_IsRead");

            migrationBuilder.RenameIndex(
                name: "IX_SupportMessages_SenderId",
                table: "SupportMessage",
                newName: "IX_SupportMessage_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportConversations_UserId",
                table: "SupportConversation",
                newName: "IX_SupportConversation_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SupportConversations_Status_CreatedAt",
                table: "SupportConversation",
                newName: "IX_SupportConversation_Status_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_SupportConversations_AssignedAdminId",
                table: "SupportConversation",
                newName: "IX_SupportConversation_AssignedAdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportMessage",
                table: "SupportMessage",
                column: "SupportMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupportConversation",
                table: "SupportConversation",
                column: "SupportConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportConversation_Users_AssignedAdminId",
                table: "SupportConversation",
                column: "AssignedAdminId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportConversation_Users_UserId",
                table: "SupportConversation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessage_SupportConversation_SupportConversationId",
                table: "SupportMessage",
                column: "SupportConversationId",
                principalTable: "SupportConversation",
                principalColumn: "SupportConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessage_Users_SenderId",
                table: "SupportMessage",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
