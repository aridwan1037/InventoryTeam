using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RelationOfSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryIdCategory",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_CategoryIdCategory",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "CategoryIdCategory",
                table: "SubCategories");

            migrationBuilder.RenameColumn(
                name: "ContactName",
                table: "Suppliers",
                newName: "ContactNumber");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Suppliers",
                newName: "EmailCompany");

            migrationBuilder.RenameColumn(
                name: "IdCategory",
                table: "SubCategories",
                newName: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "IdCategory",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategories");

            migrationBuilder.RenameColumn(
                name: "EmailCompany",
                table: "Suppliers",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "ContactNumber",
                table: "Suppliers",
                newName: "ContactName");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "SubCategories",
                newName: "IdCategory");

            migrationBuilder.AddColumn<int>(
                name: "CategoryIdCategory",
                table: "SubCategories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_CategoryIdCategory",
                table: "SubCategories",
                column: "CategoryIdCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryIdCategory",
                table: "SubCategories",
                column: "CategoryIdCategory",
                principalTable: "Categories",
                principalColumn: "IdCategory");
        }
    }
}
