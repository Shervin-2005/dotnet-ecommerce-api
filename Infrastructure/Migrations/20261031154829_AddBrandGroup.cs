using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Create BrandGroup table first.
    migrationBuilder.CreateTable(
        name: "BrandGroup",
        columns: table => new
        {
            BrandGroupId = table.Column<int>(type: "integer", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy",
                    NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Name = table.Column<string>(type: "character varying(100)",
                maxLength: 100, nullable: false),
            Slug = table.Column<string>(type: "character varying(150)",
                maxLength: 150, nullable: false),
            DisplayOrder = table.Column<int>(type: "integer", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_BrandGroup", x => x.BrandGroupId);
        });

    // 2. Create a default group for existing brands.
    migrationBuilder.InsertData(
        table: "BrandGroup",
        columns: new[] { "Name", "Slug", "DisplayOrder" },
        values: new object[] { "Other", "other", 0 });

    // 3. Add BrandGroupId as nullable temporarily.
    migrationBuilder.AddColumn<int>(
        name: "BrandGroupId",
        table: "Brands",
        type: "integer",
        nullable: true);

    // 4. Assign the default group to existing brands.
    migrationBuilder.Sql("""
        UPDATE "Brands"
        SET "BrandGroupId" = (
            SELECT "BrandGroupId"
            FROM "BrandGroup"
            WHERE "Slug" = 'other'
        );
        """);

    // 5. Make BrandGroupId required.
    migrationBuilder.AlterColumn<int>(
        name: "BrandGroupId",
        table: "Brands",
        type: "integer",
        nullable: false,
        oldClrType: typeof(int),
        oldType: "integer",
        oldNullable: true);

    // 6. Create indexes.
    migrationBuilder.CreateIndex(
        name: "IX_Brands_BrandGroupId",
        table: "Brands",
        column: "BrandGroupId");

    migrationBuilder.CreateIndex(
        name: "IX_BrandGroup_Slug",
        table: "BrandGroup",
        column: "Slug",
        unique: true);

    // 7. Add foreign key.
    migrationBuilder.AddForeignKey(
        name: "FK_Brands_BrandGroup_BrandGroupId",
        table: "Brands",
        column: "BrandGroupId",
        principalTable: "BrandGroup",
        principalColumn: "BrandGroupId",
        onDelete: ReferentialAction.Restrict);
}
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_BrandGroup_BrandGroupId",
                table: "Brands");

            migrationBuilder.DropTable(
                name: "BrandGroup");

            migrationBuilder.DropIndex(
                name: "IX_Brands_BrandGroupId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "BrandGroupId",
                table: "Brands");
        }
    }
}
