using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraints_Lookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Protocols_Name",
                table: "Protocols");

            migrationBuilder.DropIndex(
                name: "IX_MalwareFamilies_Name",
                table: "MalwareFamilies");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Code",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_AsnRegistries_Number",
                table: "AsnRegistries");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_Name",
                table: "Protocols",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MalwareFamilies_Name",
                table: "MalwareFamilies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsnRegistries_Number",
                table: "AsnRegistries",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Protocols_Name",
                table: "Protocols");

            migrationBuilder.DropIndex(
                name: "IX_MalwareFamilies_Name",
                table: "MalwareFamilies");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Code",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_AsnRegistries_Number",
                table: "AsnRegistries");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_Name",
                table: "Protocols",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MalwareFamilies_Name",
                table: "MalwareFamilies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_AsnRegistries_Number",
                table: "AsnRegistries",
                column: "Number");
        }
    }
}
