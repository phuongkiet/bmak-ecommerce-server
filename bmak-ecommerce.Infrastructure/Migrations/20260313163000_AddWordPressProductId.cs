using bmak_ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bmak_ecommerce.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260313163000_AddWordPressProductId")]
    public partial class AddWordPressProductId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WordPressProductId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_WordPressProductId",
                table: "Products",
                column: "WordPressProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_WordPressProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WordPressProductId",
                table: "Products");
        }
    }
}