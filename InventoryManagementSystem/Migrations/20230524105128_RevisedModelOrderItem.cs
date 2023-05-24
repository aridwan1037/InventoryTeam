using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class RevisedModelOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicturePath",
                table: "OrderItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PicturePath",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);
        }
    }
}
