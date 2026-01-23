using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bmak_ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SaleEndDate",
                table: "Products",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SaleStartDate",
                table: "Products",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SaleStartDate",
                table: "Products");
        }
    }
}
