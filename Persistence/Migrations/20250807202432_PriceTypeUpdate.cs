using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PriceTypeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Products",
                newName: "SalesCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                newName: "IX_Products_SalesCategoryId");

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "WorkerFunctions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "MeasurementUnitId",
                table: "WorkerFunctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WorkerFunctions_MeasurementUnitId",
                table: "WorkerFunctions",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUnits_Unit",
                table: "MeasurementUnits",
                column: "Unit",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SalesCategory_SalesCategoryId",
                table: "Products",
                column: "SalesCategoryId",
                principalTable: "SalesCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerFunctions_MeasurementUnits_MeasurementUnitId",
                table: "WorkerFunctions",
                column: "MeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SalesCategory_SalesCategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerFunctions_MeasurementUnits_MeasurementUnitId",
                table: "WorkerFunctions");

            migrationBuilder.DropIndex(
                name: "IX_WorkerFunctions_MeasurementUnitId",
                table: "WorkerFunctions");

            migrationBuilder.DropIndex(
                name: "IX_MeasurementUnits_Unit",
                table: "MeasurementUnits");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "WorkerFunctions");

            migrationBuilder.RenameColumn(
                name: "SalesCategoryId",
                table: "Products",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_SalesCategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "WorkerFunctions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Name",
                table: "ProductCategories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
