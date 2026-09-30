using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailPlatformInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEmailJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptCount",
                table: "email_jobs");

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "email_jobs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastError",
                table: "email_jobs");

            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                table: "email_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
