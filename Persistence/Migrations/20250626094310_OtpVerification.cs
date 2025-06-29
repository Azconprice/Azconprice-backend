using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "PhoneVerifications",
                newName: "Contact");

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "PhoneVerifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "PhoneVerifications");

            migrationBuilder.RenameColumn(
                name: "Contact",
                table: "PhoneVerifications",
                newName: "PhoneNumber");
        }
    }
}
