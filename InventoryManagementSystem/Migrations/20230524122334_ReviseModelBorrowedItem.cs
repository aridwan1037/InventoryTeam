using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class ReviseModelBorrowedItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LostId",
                table: "BorrowedItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LostItems",
                columns: table => new
                {
                    LostId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LostDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteItemLost = table.Column<string>(type: "TEXT", nullable: true),
                    NoteItemFound = table.Column<string>(type: "TEXT", nullable: true),
                    BorrowedId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostItems", x => x.LostId);
                    table.ForeignKey(
                        name: "FK_LostItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LostItems_BorrowedItems_BorrowedId",
                        column: x => x.BorrowedId,
                        principalTable: "BorrowedItems",
                        principalColumn: "BorrowedId");
                    table.ForeignKey(
                        name: "FK_LostItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LostItems_BorrowedId",
                table: "LostItems",
                column: "BorrowedId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LostItems_ItemId",
                table: "LostItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LostItems_UserId",
                table: "LostItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LostItems");

            migrationBuilder.DropColumn(
                name: "LostId",
                table: "BorrowedItems");
        }
    }
}
