using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NamingChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PhoneVerifications",
                table: "PhoneVerifications");

            migrationBuilder.RenameTable(
                name: "PhoneVerifications",
                newName: "OtpVerifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications");

            migrationBuilder.RenameTable(
                name: "OtpVerifications",
                newName: "PhoneVerifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhoneVerifications",
                table: "PhoneVerifications",
                column: "Id");
        }
    }
}
