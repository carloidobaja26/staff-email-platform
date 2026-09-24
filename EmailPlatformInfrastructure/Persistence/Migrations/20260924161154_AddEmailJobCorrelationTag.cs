using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailPlatformInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailJobCorrelationTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationTag",
                table: "email_jobs",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_jobs_CorrelationTag",
                table: "email_jobs",
                column: "CorrelationTag",
                unique: true,
                filter: "\"CorrelationTag\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_email_jobs_CorrelationTag",
                table: "email_jobs");

            migrationBuilder.DropColumn(
                name: "CorrelationTag",
                table: "email_jobs");
        }
    }
}
