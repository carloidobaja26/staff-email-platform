using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmailPlatformInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Tenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WebsiteName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "WebsiteName" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Development", "https://dev2.staffconnect-app.com" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Demo", "https://demo.staffconnect-app.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_email_jobs_TenantId",
                table: "email_jobs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Name",
                table: "tenants",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_email_jobs_tenants_TenantId",
                table: "email_jobs",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_email_jobs_tenants_TenantId",
                table: "email_jobs");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropIndex(
                name: "IX_email_jobs_TenantId",
                table: "email_jobs");
        }
    }
}
