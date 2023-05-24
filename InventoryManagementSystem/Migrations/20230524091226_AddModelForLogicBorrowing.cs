using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddModelForLogicBorrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_AspNetUsers_UserId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Items_ItemId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_RequestItems_RequestId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                newName: "OrderItems");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_UserId",
                table: "OrderItems",
                newName: "IX_OrderItems_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_RequestId",
                table: "OrderItems",
                newName: "IX_OrderItems_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_ItemId",
                table: "OrderItems",
                newName: "IX_OrderItems_ItemId");

            migrationBuilder.AddColumn<int>(
                name: "BorrowedId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PicturePath",
                table: "OrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateTable(
                name: "BorrowedItems",
                columns: table => new
                {
                    BorrowedId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    ReceiptId = table.Column<int>(type: "INTEGER", nullable: true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BorrowedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteBorrowed = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowedItems", x => x.BorrowedId);
                    table.ForeignKey(
                        name: "FK_BorrowedItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowedItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowedItems_OrderItems_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "GoodReceipts",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MissedDueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteItemReturned = table.Column<string>(type: "TEXT", nullable: true),
                    NoteItemLost = table.Column<string>(type: "TEXT", nullable: true),
                    NoteItemBroken = table.Column<string>(type: "TEXT", nullable: true),
                    PicturePath = table.Column<string>(type: "TEXT", nullable: true),
                    BorrowedId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodReceipts", x => x.ReceiptId);
                    table.ForeignKey(
                        name: "FK_GoodReceipts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoodReceipts_BorrowedItems_BorrowedId",
                        column: x => x.BorrowedId,
                        principalTable: "BorrowedItems",
                        principalColumn: "BorrowedId");
                    table.ForeignKey(
                        name: "FK_GoodReceipts_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedItems_ItemId",
                table: "BorrowedItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedItems_OrderId",
                table: "BorrowedItems",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BorrowedItems_UserId",
                table: "BorrowedItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodReceipts_BorrowedId",
                table: "GoodReceipts",
                column: "BorrowedId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodReceipts_ItemId",
                table: "GoodReceipts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodReceipts_UserId",
                table: "GoodReceipts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_AspNetUsers_UserId",
                table: "OrderItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Items_ItemId",
                table: "OrderItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "IdItem",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_RequestItems_RequestId",
                table: "OrderItems",
                column: "RequestId",
                principalTable: "RequestItems",
                principalColumn: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_AspNetUsers_UserId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Items_ItemId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_RequestItems_RequestId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "GoodReceipts");

            migrationBuilder.DropTable(
                name: "BorrowedItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BorrowedId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PicturePath",
                table: "OrderItems");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_UserId",
                table: "OrderItem",
                newName: "IX_OrderItem_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_RequestId",
                table: "OrderItem",
                newName: "IX_OrderItem_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItem",
                newName: "IX_OrderItem_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_AspNetUsers_UserId",
                table: "OrderItem",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Items_ItemId",
                table: "OrderItem",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "IdItem",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_RequestItems_RequestId",
                table: "OrderItem",
                column: "RequestId",
                principalTable: "RequestItems",
                principalColumn: "RequestId");
        }
    }
}
