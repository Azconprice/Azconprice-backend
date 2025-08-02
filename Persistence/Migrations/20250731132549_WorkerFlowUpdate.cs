using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkerFlowUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkerSpecializations");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyProfileId",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "WorkerFunctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerFunctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerFunctions_Professions_ProfessionId",
                        column: x => x.ProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerFunctions_WorkerProfiles_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerFunctionSpecialization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerFunctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerFunctionSpecialization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerFunctionSpecialization_Specializations_Specialization~",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerFunctionSpecialization_WorkerFunctions_WorkerFunction~",
                        column: x => x.WorkerFunctionId,
                        principalTable: "WorkerFunctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyProfileId",
                table: "Products",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerFunctions_ProfessionId",
                table: "WorkerFunctions",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerFunctions_WorkerProfileId",
                table: "WorkerFunctions",
                column: "WorkerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerFunctionSpecialization_SpecializationId",
                table: "WorkerFunctionSpecialization",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerFunctionSpecialization_WorkerFunctionId",
                table: "WorkerFunctionSpecialization",
                column: "WorkerFunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CompanyProfiles_CompanyProfileId",
                table: "Products",
                column: "CompanyProfileId",
                principalTable: "CompanyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CompanyProfiles_CompanyProfileId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "WorkerFunctionSpecialization");

            migrationBuilder.DropTable(
                name: "WorkerFunctions");

            migrationBuilder.DropIndex(
                name: "IX_Products_CompanyProfileId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CompanyProfileId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "WorkerSpecializations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecializationId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkerSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerSpecializations_WorkerProfiles_WorkerProfileId",
                        column: x => x.WorkerProfileId,
                        principalTable: "WorkerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerSpecializations_SpecializationId",
                table: "WorkerSpecializations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerSpecializations_WorkerProfileId",
                table: "WorkerSpecializations",
                column: "WorkerProfileId");
        }
    }
}
