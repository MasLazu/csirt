using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantUserPasswordWithCorrectTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantUserLoginMethods_LoginMethods_LoginMethodId",
                table: "TenantUserLoginMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantUserLoginMethods_TenantUsers_TenantUserId",
                table: "TenantUserLoginMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantUserLoginMethods",
                table: "TenantUserLoginMethods");

            migrationBuilder.RenameTable(
                name: "TenantUserLoginMethods",
                newName: "tenant_user_login_methods");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tenant_user_login_methods",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "tenant_user_login_methods",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TenantUserId",
                table: "tenant_user_login_methods",
                newName: "tenant_user_id");

            migrationBuilder.RenameColumn(
                name: "LoginMethodId",
                table: "tenant_user_login_methods",
                newName: "login_method_id");

            migrationBuilder.RenameColumn(
                name: "LoginMethodCode",
                table: "tenant_user_login_methods",
                newName: "login_method_code");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "tenant_user_login_methods",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "tenant_user_login_methods",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUserLoginMethods_TenantUserId_LoginMethodCode",
                table: "tenant_user_login_methods",
                newName: "IX_tenant_user_login_methods_tenant_user_id_login_method_code");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUserLoginMethods_TenantUserId",
                table: "tenant_user_login_methods",
                newName: "IX_tenant_user_login_methods_tenant_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUserLoginMethods_LoginMethodId",
                table: "tenant_user_login_methods",
                newName: "IX_tenant_user_login_methods_login_method_id");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUserLoginMethods_DeletedAt",
                table: "tenant_user_login_methods",
                newName: "IX_tenant_user_login_methods_deleted_at");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenant_user_login_methods",
                table: "tenant_user_login_methods",
                column: "id");

            migrationBuilder.CreateTable(
                name: "tenant_user_passwords",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_user_login_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_salt = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_user_passwords", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_user_passwords_tenant_user_login_methods_tenant_user~",
                        column: x => x.tenant_user_login_method_id,
                        principalTable: "tenant_user_login_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_user_passwords_tenant_user_login_method_id",
                table: "tenant_user_passwords",
                column: "tenant_user_login_method_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_user_login_methods_LoginMethods_login_method_id",
                table: "tenant_user_login_methods",
                column: "login_method_id",
                principalTable: "LoginMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_user_login_methods_TenantUsers_tenant_user_id",
                table: "tenant_user_login_methods",
                column: "tenant_user_id",
                principalTable: "TenantUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_user_login_methods_LoginMethods_login_method_id",
                table: "tenant_user_login_methods");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_user_login_methods_TenantUsers_tenant_user_id",
                table: "tenant_user_login_methods");

            migrationBuilder.DropTable(
                name: "tenant_user_passwords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenant_user_login_methods",
                table: "tenant_user_login_methods");

            migrationBuilder.RenameTable(
                name: "tenant_user_login_methods",
                newName: "TenantUserLoginMethods");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TenantUserLoginMethods",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "TenantUserLoginMethods",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "tenant_user_id",
                table: "TenantUserLoginMethods",
                newName: "TenantUserId");

            migrationBuilder.RenameColumn(
                name: "login_method_id",
                table: "TenantUserLoginMethods",
                newName: "LoginMethodId");

            migrationBuilder.RenameColumn(
                name: "login_method_code",
                table: "TenantUserLoginMethods",
                newName: "LoginMethodCode");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "TenantUserLoginMethods",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "TenantUserLoginMethods",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_user_login_methods_tenant_user_id_login_method_code",
                table: "TenantUserLoginMethods",
                newName: "IX_TenantUserLoginMethods_TenantUserId_LoginMethodCode");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_user_login_methods_tenant_user_id",
                table: "TenantUserLoginMethods",
                newName: "IX_TenantUserLoginMethods_TenantUserId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_user_login_methods_login_method_id",
                table: "TenantUserLoginMethods",
                newName: "IX_TenantUserLoginMethods_LoginMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_tenant_user_login_methods_deleted_at",
                table: "TenantUserLoginMethods",
                newName: "IX_TenantUserLoginMethods_DeletedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantUserLoginMethods",
                table: "TenantUserLoginMethods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUserLoginMethods_LoginMethods_LoginMethodId",
                table: "TenantUserLoginMethods",
                column: "LoginMethodId",
                principalTable: "LoginMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUserLoginMethods_TenantUsers_TenantUserId",
                table: "TenantUserLoginMethods",
                column: "TenantUserId",
                principalTable: "TenantUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
