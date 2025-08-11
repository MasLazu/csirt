using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSOmeTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantAsns_AsnInfos_AsnRegistryId",
                table: "TenantAsns");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantAsns_Tenants_TenantId",
                table: "TenantAsns");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreatEvents_AsnInfos_AsnRegistryId",
                table: "ThreatEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantAsns",
                table: "TenantAsns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AsnInfos",
                table: "AsnInfos");

            migrationBuilder.RenameTable(
                name: "TenantAsns",
                newName: "TenantAsnRegistries");

            migrationBuilder.RenameTable(
                name: "AsnInfos",
                newName: "AsnRegistries");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsns_TenantId_AsnRegistryId",
                table: "TenantAsnRegistries",
                newName: "IX_TenantAsnRegistries_TenantId_AsnRegistryId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsns_TenantId",
                table: "TenantAsnRegistries",
                newName: "IX_TenantAsnRegistries_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsns_DeletedAt",
                table: "TenantAsnRegistries",
                newName: "IX_TenantAsnRegistries_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsns_AsnRegistryId",
                table: "TenantAsnRegistries",
                newName: "IX_TenantAsnRegistries_AsnRegistryId");

            migrationBuilder.RenameIndex(
                name: "IX_AsnInfos_Number",
                table: "AsnRegistries",
                newName: "IX_AsnRegistries_Number");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantAsnRegistries",
                table: "TenantAsnRegistries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AsnRegistries",
                table: "AsnRegistries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantAsnRegistries_AsnRegistries_AsnRegistryId",
                table: "TenantAsnRegistries",
                column: "AsnRegistryId",
                principalTable: "AsnRegistries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantAsnRegistries_Tenants_TenantId",
                table: "TenantAsnRegistries",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreatEvents_AsnRegistries_AsnRegistryId",
                table: "ThreatEvents",
                column: "AsnRegistryId",
                principalTable: "AsnRegistries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantAsnRegistries_AsnRegistries_AsnRegistryId",
                table: "TenantAsnRegistries");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantAsnRegistries_Tenants_TenantId",
                table: "TenantAsnRegistries");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreatEvents_AsnRegistries_AsnRegistryId",
                table: "ThreatEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantAsnRegistries",
                table: "TenantAsnRegistries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AsnRegistries",
                table: "AsnRegistries");

            migrationBuilder.RenameTable(
                name: "TenantAsnRegistries",
                newName: "TenantAsns");

            migrationBuilder.RenameTable(
                name: "AsnRegistries",
                newName: "AsnInfos");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsnRegistries_TenantId_AsnRegistryId",
                table: "TenantAsns",
                newName: "IX_TenantAsns_TenantId_AsnRegistryId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsnRegistries_TenantId",
                table: "TenantAsns",
                newName: "IX_TenantAsns_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsnRegistries_DeletedAt",
                table: "TenantAsns",
                newName: "IX_TenantAsns_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_TenantAsnRegistries_AsnRegistryId",
                table: "TenantAsns",
                newName: "IX_TenantAsns_AsnRegistryId");

            migrationBuilder.RenameIndex(
                name: "IX_AsnRegistries_Number",
                table: "AsnInfos",
                newName: "IX_AsnInfos_Number");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantAsns",
                table: "TenantAsns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AsnInfos",
                table: "AsnInfos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantAsns_AsnInfos_AsnRegistryId",
                table: "TenantAsns",
                column: "AsnRegistryId",
                principalTable: "AsnInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantAsns_Tenants_TenantId",
                table: "TenantAsns",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreatEvents_AsnInfos_AsnRegistryId",
                table: "ThreatEvents",
                column: "AsnRegistryId",
                principalTable: "AsnInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
