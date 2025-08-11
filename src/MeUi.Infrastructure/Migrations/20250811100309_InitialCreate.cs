using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.UniqueConstraint("AK_Actions_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AsnInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsnInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginMethods", x => x.Id);
                    table.UniqueConstraint("AK_LoginMethods_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MalwareFamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MalwareFamilies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PasswordSalt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Protocols",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.UniqueConstraint("AK_Resources_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PageGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_PageGroups_PageGroupId",
                        column: x => x.PageGroupId,
                        principalTable: "PageGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ThreatEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AsnRegistryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceAddress = table.Column<IPAddress>(type: "inet", nullable: false),
                    SourceCountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationAddress = table.Column<IPAddress>(type: "inet", nullable: true),
                    DestinationCountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourcePort = table.Column<int>(type: "integer", nullable: true),
                    DestinationPort = table.Column<int>(type: "integer", nullable: true),
                    ProtocolId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MalwareFamilyId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatEvents", x => new { x.Id, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_ThreatEvents_AsnInfos_AsnRegistryId",
                        column: x => x.AsnRegistryId,
                        principalTable: "AsnInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreatEvents_Countries_DestinationCountryId",
                        column: x => x.DestinationCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThreatEvents_Countries_SourceCountryId",
                        column: x => x.SourceCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThreatEvents_MalwareFamilies_MalwareFamilyId",
                        column: x => x.MalwareFamilyId,
                        principalTable: "MalwareFamilies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ThreatEvents_Protocols_ProtocolId",
                        column: x => x.ProtocolId,
                        principalTable: "Protocols",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Actions_ActionCode",
                        column: x => x.ActionCode,
                        principalTable: "Actions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_Actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Actions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Permissions_Resources_ResourceCode",
                        column: x => x.ResourceCode,
                        principalTable: "Resources",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ActionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantPermissions_Actions_ActionCode",
                        column: x => x.ActionCode,
                        principalTable: "Actions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantPermissions_Resources_ResourceCode",
                        column: x => x.ResourceCode,
                        principalTable: "Resources",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantAsns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AsnRegistryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantAsns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantAsns_AsnInfos_AsnRegistryId",
                        column: x => x.AsnRegistryId,
                        principalTable: "AsnInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantAsns_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRole_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsSuspended = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUsers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginMethodCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginMethods_LoginMethods_LoginMethodCode",
                        column: x => x.LoginMethodCode,
                        principalTable: "LoginMethods",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLoginMethods_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshTokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_RefreshTokens_RefreshTokenId",
                        column: x => x.RefreshTokenId,
                        principalTable: "RefreshTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagePermissions_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageTenantPermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PageId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantPermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTenantPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTenantPermission_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageTenantPermission_TenantPermissions_TenantPermissionId",
                        column: x => x.TenantPermissionId,
                        principalTable: "TenantPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantRolePermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantPermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRolePermission_TenantPermissions_TenantPermissionId",
                        column: x => x.TenantPermissionId,
                        principalTable: "TenantPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantRolePermission_TenantRole_TenantRoleId",
                        column: x => x.TenantRoleId,
                        principalTable: "TenantRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserLoginMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginMethodCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserLoginMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserLoginMethods_LoginMethods_LoginMethodCode",
                        column: x => x.LoginMethodCode,
                        principalTable: "LoginMethods",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserLoginMethods_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserRefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshTokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserRefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserRefreshTokens_RefreshTokens_RefreshTokenId",
                        column: x => x.RefreshTokenId,
                        principalTable: "RefreshTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserRefreshTokens_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantRoleId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantRole_TenantRoleId",
                        column: x => x.TenantRoleId,
                        principalTable: "TenantRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantRole_TenantRoleId1",
                        column: x => x.TenantRoleId1,
                        principalTable: "TenantRole",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserLoginMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPasswords_Passwords_PasswordId",
                        column: x => x.PasswordId,
                        principalTable: "Passwords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPasswords_UserLoginMethods_UserLoginMethodId",
                        column: x => x.UserLoginMethodId,
                        principalTable: "UserLoginMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserPasswords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantUserLoginMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserPasswords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserPasswords_Passwords_PasswordId",
                        column: x => x.PasswordId,
                        principalTable: "Passwords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserPasswords_TenantUserLoginMethods_TenantUserLoginM~",
                        column: x => x.TenantUserLoginMethodId,
                        principalTable: "TenantUserLoginMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserPasswords_TenantUsers_TenantUserId",
                        column: x => x.TenantUserId,
                        principalTable: "TenantUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_Code",
                table: "Actions",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_DeletedAt",
                table: "Actions",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AsnInfos_Number",
                table: "AsnInfos",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_DeletedAt",
                table: "Countries",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LoginMethods_Code",
                table: "LoginMethods",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_LoginMethods_DeletedAt",
                table: "LoginMethods",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MalwareFamilies_DeletedAt",
                table: "MalwareFamilies",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MalwareFamilies_Name",
                table: "MalwareFamilies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PageGroups_Code",
                table: "PageGroups",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_PageGroups_DeletedAt",
                table: "PageGroups",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PagePermissions_DeletedAt",
                table: "PagePermissions",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PagePermissions_PageId",
                table: "PagePermissions",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PagePermissions_PermissionId",
                table: "PagePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_Code",
                table: "Pages",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_DeletedAt",
                table: "Pages",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageGroupId",
                table: "Pages",
                column: "PageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_ParentId",
                table: "Pages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_Path",
                table: "Pages",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_PageTenantPermission_DeletedAt",
                table: "PageTenantPermission",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PageTenantPermission_PageId",
                table: "PageTenantPermission",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTenantPermission_TenantPermissionId",
                table: "PageTenantPermission",
                column: "TenantPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Passwords_DeletedAt",
                table: "Passwords",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ActionCode",
                table: "Permissions",
                column: "ActionCode");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ActionId",
                table: "Permissions",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_DeletedAt",
                table: "Permissions",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ResourceCode_ActionCode",
                table: "Permissions",
                columns: new[] { "ResourceCode", "ActionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_DeletedAt",
                table: "Protocols",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_Name",
                table: "Protocols",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DeletedAt",
                table: "RefreshTokens",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Code",
                table: "Resources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_DeletedAt",
                table: "Resources",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_DeletedAt",
                table: "RolePermissions",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DeletedAt",
                table: "Roles",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAsns_AsnRegistryId",
                table: "TenantAsns",
                column: "AsnRegistryId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAsns_DeletedAt",
                table: "TenantAsns",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAsns_TenantId",
                table: "TenantAsns",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAsns_TenantId_AsnRegistryId",
                table: "TenantAsns",
                columns: new[] { "TenantId", "AsnRegistryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantPermissions_ActionCode",
                table: "TenantPermissions",
                column: "ActionCode");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPermissions_DeletedAt",
                table: "TenantPermissions",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPermissions_ResourceCode_ActionCode",
                table: "TenantPermissions",
                columns: new[] { "ResourceCode", "ActionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantRole_DeletedAt",
                table: "TenantRole",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRole_Name",
                table: "TenantRole",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRole_TenantId",
                table: "TenantRole",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRolePermission_TenantPermissionId",
                table: "TenantRolePermission",
                column: "TenantPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRolePermission_TenantRoleId",
                table: "TenantRolePermission",
                column: "TenantRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ContactEmail",
                table: "Tenants",
                column: "ContactEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_DeletedAt",
                table: "Tenants",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_IsActive",
                table: "Tenants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Name",
                table: "Tenants",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserLoginMethods_DeletedAt",
                table: "TenantUserLoginMethods",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserLoginMethods_LoginMethodCode",
                table: "TenantUserLoginMethods",
                column: "LoginMethodCode");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserLoginMethods_TenantUserId",
                table: "TenantUserLoginMethods",
                column: "TenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserLoginMethods_TenantUserId_LoginMethodCode",
                table: "TenantUserLoginMethods",
                columns: new[] { "TenantUserId", "LoginMethodCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserPasswords_DeletedAt",
                table: "TenantUserPasswords",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserPasswords_PasswordId",
                table: "TenantUserPasswords",
                column: "PasswordId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserPasswords_TenantUserId",
                table: "TenantUserPasswords",
                column: "TenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserPasswords_TenantUserLoginMethodId",
                table: "TenantUserPasswords",
                column: "TenantUserLoginMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRefreshTokens_DeletedAt",
                table: "TenantUserRefreshTokens",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRefreshTokens_RefreshTokenId",
                table: "TenantUserRefreshTokens",
                column: "RefreshTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRefreshTokens_TenantUserId_RefreshTokenId",
                table: "TenantUserRefreshTokens",
                columns: new[] { "TenantUserId", "RefreshTokenId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_DeletedAt",
                table: "TenantUserRoles",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantRoleId",
                table: "TenantUserRoles",
                column: "TenantRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantRoleId1",
                table: "TenantUserRoles",
                column: "TenantRoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantUserId",
                table: "TenantUserRoles",
                column: "TenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_TenantUserId_TenantRoleId",
                table: "TenantUserRoles",
                columns: new[] { "TenantUserId", "TenantRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_DeletedAt",
                table: "TenantUsers",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Email_TenantId",
                table: "TenantUsers",
                columns: new[] { "Email", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_TenantId",
                table: "TenantUsers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUsers_Username_TenantId",
                table: "TenantUsers",
                columns: new[] { "Username", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_AsnRegistryId",
                table: "ThreatEvents",
                column: "AsnRegistryId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_Category",
                table: "ThreatEvents",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_DeletedAt",
                table: "ThreatEvents",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_DestinationAddress",
                table: "ThreatEvents",
                column: "DestinationAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_DestinationCountryId",
                table: "ThreatEvents",
                column: "DestinationCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_MalwareFamilyId",
                table: "ThreatEvents",
                column: "MalwareFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_ProtocolId",
                table: "ThreatEvents",
                column: "ProtocolId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_SourceAddress",
                table: "ThreatEvents",
                column: "SourceAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatEvents_SourceCountryId",
                table: "ThreatEvents",
                column: "SourceCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginMethods_DeletedAt",
                table: "UserLoginMethods",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginMethods_LoginMethodCode",
                table: "UserLoginMethods",
                column: "LoginMethodCode");

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginMethods_UserId_LoginMethodCode",
                table: "UserLoginMethods",
                columns: new[] { "UserId", "LoginMethodCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_DeletedAt",
                table: "UserPasswords",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_PasswordId",
                table: "UserPasswords",
                column: "PasswordId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_UserLoginMethodId",
                table: "UserPasswords",
                column: "UserLoginMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_DeletedAt",
                table: "UserRefreshTokens",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_RefreshTokenId",
                table: "UserRefreshTokens",
                column: "RefreshTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_UserId_RefreshTokenId",
                table: "UserRefreshTokens",
                columns: new[] { "UserId", "RefreshTokenId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_DeletedAt",
                table: "UserRoles",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedAt",
                table: "Users",
                column: "DeletedAt");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagePermissions");

            migrationBuilder.DropTable(
                name: "PageTenantPermission");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TenantAsns");

            migrationBuilder.DropTable(
                name: "TenantRolePermission");

            migrationBuilder.DropTable(
                name: "TenantUserPasswords");

            migrationBuilder.DropTable(
                name: "TenantUserRefreshTokens");

            migrationBuilder.DropTable(
                name: "TenantUserRoles");

            migrationBuilder.DropTable(
                name: "ThreatEvents");

            migrationBuilder.DropTable(
                name: "UserPasswords");

            migrationBuilder.DropTable(
                name: "UserRefreshTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "TenantPermissions");

            migrationBuilder.DropTable(
                name: "TenantUserLoginMethods");

            migrationBuilder.DropTable(
                name: "TenantRole");

            migrationBuilder.DropTable(
                name: "AsnInfos");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "MalwareFamilies");

            migrationBuilder.DropTable(
                name: "Protocols");

            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "UserLoginMethods");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "PageGroups");

            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "TenantUsers");

            migrationBuilder.DropTable(
                name: "LoginMethods");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
