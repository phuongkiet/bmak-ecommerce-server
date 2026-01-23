using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bmak_ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductsTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_ProductAttribute_AttributeId",
                table: "ProductAttributeValue");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductAttributeValue",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraData",
                table: "ProductAttributeValue",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductAttribute",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductAttribute",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductId_AttributeId",
                table: "ProductAttributeValue",
                columns: new[] { "ProductId", "AttributeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_Code",
                table: "ProductAttribute",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_Name",
                table: "ProductAttribute",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_ProductAttribute_AttributeId",
                table: "ProductAttributeValue",
                column: "AttributeId",
                principalTable: "ProductAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValue_ProductAttribute_AttributeId",
                table: "ProductAttributeValue");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValue_ProductId_AttributeId",
                table: "ProductAttributeValue");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttribute_Code",
                table: "ProductAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttribute_Name",
                table: "ProductAttribute");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductAttributeValue",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ProductAttributeValue",
                keyColumn: "ExtraData",
                keyValue: null,
                column: "ExtraData",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraData",
                table: "ProductAttributeValue",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductAttribute",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ProductAttribute",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValue_ProductAttribute_AttributeId",
                table: "ProductAttributeValue",
                column: "AttributeId",
                principalTable: "ProductAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
