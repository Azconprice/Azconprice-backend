using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompnaySalesCategoryAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "CompanyProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SalesCategoryId",
                table: "CompanyProfiles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_SalesCategoryId",
                table: "CompanyProfiles",
                column: "SalesCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProfiles_SalesCategory_SalesCategoryId",
                table: "CompanyProfiles",
                column: "SalesCategoryId",
                principalTable: "SalesCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProfiles_SalesCategory_SalesCategoryId",
                table: "CompanyProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfiles_SalesCategoryId",
                table: "CompanyProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "CompanyProfiles");

            migrationBuilder.DropColumn(
                name: "SalesCategoryId",
                table: "CompanyProfiles");
        }
    }
}
