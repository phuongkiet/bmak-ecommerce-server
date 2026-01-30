using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bmak_ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderItemAndProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderItem",
                newName: "Price");

            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "OrderItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProductSku",
                table: "OrderItem",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "QuantityOnHand",
                table: "OrderItem",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "ProductSku",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "QuantityOnHand",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderItem",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Products_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
