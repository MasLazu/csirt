using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateUserUniqueIndexForActiveUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "IX_Users_Username",
            table: "Users");

        migrationBuilder.AddColumn<Guid>(
            name: "AsnRegistryId1",
            table: "TenantAsnRegistries",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true,
            filter: "\"DeletedAt\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Username",
            table: "Users",
            column: "Username",
            unique: true,
            filter: "\"DeletedAt\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_TenantAsnRegistries_AsnRegistryId1",
            table: "TenantAsnRegistries",
            column: "AsnRegistryId1");

        migrationBuilder.AddForeignKey(
            name: "FK_TenantAsnRegistries_AsnRegistries_AsnRegistryId1",
            table: "TenantAsnRegistries",
            column: "AsnRegistryId1",
            principalTable: "AsnRegistries",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_TenantAsnRegistries_AsnRegistries_AsnRegistryId1",
            table: "TenantAsnRegistries");

        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "IX_Users_Username",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "IX_TenantAsnRegistries_AsnRegistryId1",
            table: "TenantAsnRegistries");

        migrationBuilder.DropColumn(
            name: "AsnRegistryId1",
            table: "TenantAsnRegistries");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true,
            filter: "\"Email\" IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Username",
            table: "Users",
            column: "Username",
            unique: true,
            filter: "\"Username\" IS NOT NULL");
    }
}
