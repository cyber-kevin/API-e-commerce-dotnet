using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RO.DevTest.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemSale_Sale_SaleId",
                table: "ItemSale");

            migrationBuilder.DropForeignKey(
                name: "FK_Sale_Customer_CustomerId",
                table: "Sale");

            migrationBuilder.DropForeignKey(
                name: "FK_Sale_Products_ProductId",
                table: "Sale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sale",
                table: "Sale");

            migrationBuilder.DropIndex(
                name: "IX_Sale_ProductId",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Sale");

            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "Sale");

            migrationBuilder.RenameTable(
                name: "Sale",
                newName: "Sales");

            migrationBuilder.RenameIndex(
                name: "IX_Sale_CustomerId",
                table: "Sales",
                newName: "IX_Sales_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sales",
                table: "Sales",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSale_Sales_SaleId",
                table: "ItemSale",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customer_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemSale_Sales_SaleId",
                table: "ItemSale");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customer_CustomerId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sales",
                table: "Sales");

            migrationBuilder.RenameTable(
                name: "Sales",
                newName: "Sale");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_CustomerId",
                table: "Sale",
                newName: "IX_Sale_CustomerId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Sale",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Sale",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "Sale",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sale",
                table: "Sale",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_ProductId",
                table: "Sale",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSale_Sale_SaleId",
                table: "ItemSale",
                column: "SaleId",
                principalTable: "Sale",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_Customer_CustomerId",
                table: "Sale",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_Products_ProductId",
                table: "Sale",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
