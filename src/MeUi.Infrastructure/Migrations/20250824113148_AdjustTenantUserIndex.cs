using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustTenantUserIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantUsers_Email_TenantId",
                table: "TenantUsers");

            migrationBuilder.DropIndex(
                name: "IX_TenantUsers_Username_TenantId",
                table: "TenantUsers");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Email",
                table: "TenantUsers",
                column: "Email",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Username",
                table: "TenantUsers",
                column: "Username",
                unique: true,
                filter: "\"DeletedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantUsers_Email",
                table: "TenantUsers");

            migrationBuilder.DropIndex(
                name: "IX_TenantUsers_Username",
                table: "TenantUsers");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Email_TenantId",
                table: "TenantUsers",
                columns: new[] { "Email", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Username_TenantId",
                table: "TenantUsers",
                columns: new[] { "Username", "TenantId" },
                unique: true);
        }
    }
}
