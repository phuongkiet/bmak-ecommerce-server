using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bmak_ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refactor_ProductAttribute_SharedValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_Products_ProductId",
                table: "ProductAttributeValue");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValue_ProductId_AttributeId",
                table: "ProductAttributeValue");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductAttributeValue");

            migrationBuilder.CreateTable(
                name: "ProductAttributeSelection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    AttributeValueId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeSelection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeSelection_ProductAttributeValue_AttributeVal~",
                        column: x => x.AttributeValueId,
                        principalTable: "ProductAttributeValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeSelection_ProductAttribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttributeSelection_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_AttributeId_Value",
                table: "ProductAttributeValue",
                columns: new[] { "AttributeId", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeSelection_AttributeId",
                table: "ProductAttributeSelection",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeSelection_AttributeValueId",
                table: "ProductAttributeSelection",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeSelection_ProductId",
                table: "ProductAttributeSelection",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeSelection_ProductId_AttributeId",
                table: "ProductAttributeSelection",
                columns: new[] { "ProductId", "AttributeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttributeSelection");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValue_AttributeId_Value",
                table: "ProductAttributeValue");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductAttributeValue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductId",
                table: "ProductAttributeValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductId_AttributeId",
                table: "ProductAttributeValue",
                columns: new[] { "ProductId", "AttributeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_Products_ProductId",
                table: "ProductAttributeValue",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
