using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class PembuatanSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubCategoryId",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CategoryCode",
                table: "Categories",
                type: "TEXT",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    IdSubCategory = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubCategoryCode = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    SubCategoryName = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    IdCategory = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryIdCategory = table.Column<int>(type: "INTEGER", nullable: true),
                    Description = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.IdSubCategory);
                    table.ForeignKey(
                        name: "FK_SubCategories_Categories_CategoryIdCategory",
                        column: x => x.CategoryIdCategory,
                        principalTable: "Categories",
                        principalColumn: "IdCategory");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_SubCategoryId",
                table: "Items",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryIdCategory",
                table: "SubCategories",
                column: "CategoryIdCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId",
                table: "Items",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "IdSubCategory",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubCategories_SubCategoryId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_Items_SubCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CategoryCode",
                table: "Categories");
        }
    }
}
