using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailPlatformInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailDeliveryLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_email_deliveries_Provider_ProviderMessageId",
                table: "email_deliveries",
                columns: new[] { "Provider", "ProviderMessageId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_email_deliveries_Provider_ProviderMessageId",
                table: "email_deliveries");
        }
    }
}
