using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailPlatformInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailJobRetryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                table: "email_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxAttempts",
                table: "email_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextAttemptAt",
                table: "email_jobs",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptCount",
                table: "email_jobs");

            migrationBuilder.DropColumn(
                name: "MaxAttempts",
                table: "email_jobs");

            migrationBuilder.DropColumn(
                name: "NextAttemptAt",
                table: "email_jobs");
        }
    }
}
