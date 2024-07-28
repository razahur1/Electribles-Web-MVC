using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace electrible.Migrations
{
    /// <inheritdoc />
    public partial class order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cartitems_Cart_CartId",
                table: "Cartitems");

            migrationBuilder.DropForeignKey(
                name: "FK_Cartitems_Products_ProductId",
                table: "Cartitems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cartitems",
                table: "Cartitems");

            migrationBuilder.RenameTable(
                name: "Cartitems",
                newName: "CartItem");

            migrationBuilder.RenameIndex(
                name: "IX_Cartitems_ProductId",
                table: "CartItem",
                newName: "IX_CartItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Cartitems_CartId",
                table: "CartItem",
                newName: "IX_CartItem_CartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem",
                column: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Cart_CartId",
                table: "CartItem",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Products_ProductId",
                table: "CartItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Cart_CartId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Products_ProductId",
                table: "CartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem");

            migrationBuilder.RenameTable(
                name: "CartItem",
                newName: "Cartitems");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_ProductId",
                table: "Cartitems",
                newName: "IX_Cartitems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItem_CartId",
                table: "Cartitems",
                newName: "IX_Cartitems_CartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cartitems",
                table: "Cartitems",
                column: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartitems_Cart_CartId",
                table: "Cartitems",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cartitems_Products_ProductId",
                table: "Cartitems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
