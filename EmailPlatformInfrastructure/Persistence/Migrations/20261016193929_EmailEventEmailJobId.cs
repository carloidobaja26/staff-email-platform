using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailPlatformInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailEventEmailJobId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmailJobId",
                table: "email_webhook_events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_email_webhook_events_EmailJobId",
                table: "email_webhook_events",
                column: "EmailJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_email_webhook_events_email_jobs_EmailJobId",
                table: "email_webhook_events",
                column: "EmailJobId",
                principalTable: "email_jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_email_webhook_events_email_jobs_EmailJobId",
                table: "email_webhook_events");

            migrationBuilder.DropIndex(
                name: "IX_email_webhook_events_EmailJobId",
                table: "email_webhook_events");

            migrationBuilder.DropColumn(
                name: "EmailJobId",
                table: "email_webhook_events");
        }
    }
}
