# Endpoint Permission Provider Migration Tracker

This document tracks the migration of endpoint classes to use the new base endpoint types for permission and tenant permission providers.

## Migration Goals

- Migrate all endpoints implementing `IPermissionProvider` to use:
  - `BaseAuthorizedEndpoint`
  - `BaseAuthorizedEndpointWithoutRequest`
  - `BaseAuthorizedEndpointWithoutResponse`
  - `BaseAuthorizedEndpointWithourRequestResponse`
- Migrate all endpoints implementing both `IPermissionProvider` and `ITenantPermissionProvider` to use:
  - `BaseTenantAuthorizedEndpoint`
  - `BaseTenantAuthorizedEndpointWithoutResponse`

## Example Implementations

- `CreateAsnRegistryEndpoint`
- `DeleteAsnRegistryEndpoint`
- `GetAsnRegistriesEndpoint`

---

## Migration Progress

Migration Progress (File System Structure):

src/MeUi.Api/Endpoints/

src/MeUi.Api/Endpoints/
├── AsnRegistries
│   ├── CreateAsnRegistryEndpoint.cs [Migrated]
│   ├── DeleteAsnRegistryEndpoint.cs [Migrated]
│   ├── GetAsnRegistriesEndpoint.cs [Migrated]
│   ├── GetAsnRegistryByIdEndpoint.cs [Migrated]
│   └── UpdateAsnRegistryEndpoint.cs [Migrated]
├── Authentication
│   ├── LoginEndpoint.cs
│   ├── LoginMethod
│   │   ├── GetActiveLoginMethodsEndpoint.cs
│   │   └── GetLoginMethodsEndpoint.cs
│   ├── LogoutEndpoint.cs
│   └── RefreshTokenEndpoint.cs
├── Authorization
│   ├── Action
│   │   └── GetActionsEndpoint.cs
│   ├── Me
│   │   ├── GetAccessiblePageGroupsEndpoint.cs
│   │   ├── GetUserPermissionsEndpoint.cs [Migrated]
│   │   └── GetUserRolesMeEndpoint.cs
│   ├── Page
│   │   └── GetPagesEndpoint.cs [Pending]
│   ├── PageGroup
│   │   └── GetPageGroupsEndpoint.cs
│   ├── Permission
│   │   └── GetPermissionsEndpoint.cs
│   ├── Resource
│   │   └── GetResourcesEndpoint.cs [Pending]
│   ├── Role
│   │   ├── CreateRoleEndpoint.cs
│   │   ├── DeleteRoleEndpoint.cs
│   │   ├── GetRolesPaginatedEndpoint.cs
│   │   └── UpdateRoleEndpoint.cs
│   ├── User
│   │   ├── GetUserAccessiblePageGroupsEndpoint.cs [Migrated]
├── Page [Migrated]
│ └── GetPagesEndpoint.cs [Migrated]
│   ├── CreateUserRoleEndpoint.cs [Migrated]
│   ├── DeleteUserRoleEndpoint.cs [Migrated]
│   ├── GetUserRolesEndpoint.cs [Migrated]
│   └── PutUserRolesEndpoint.cs [Migrated]
├── Countries
│   ├── CreateCountryEndpoint.cs
├── Resource [Migrated]
│ └── GetResourcesEndpoint.cs [Migrated]
│   ├── GetCountryByIdEndpoint.cs
│   └── UpdateCountryEndpoint.cs
├── MalwareFamilies
│   ├── CreateMalwareFamilyEndpoint.cs [Migrated]
│   ├── DeleteMalwareFamilyEndpoint.cs [Migrated]
│   ├── GetMalwareFamiliesEndpoint.cs [Migrated]
│   ├── GetMalwareFamilyByIdEndpoint.cs [Migrated]
│   └── UpdateMalwareFamilyEndpoint.cs [Migrated]
├── Protocols
│   ├── CreateProtocolEndpoint.cs [Migrated]
│   ├── DeleteProtocolEndpoint.cs [Migrated]
│   ├── GetProtocolByIdEndpoint.cs [Migrated]
│   ├── GetProtocolsEndpoint.cs [Migrated]
│   └── UpdateProtocolEndpoint.cs [Migrated]
├── TenantAsnRegistries
│   ├── AssignAsnRegistriesToTenantEndpoint.cs
│   ├── BulkAssignAsnRegistriesToTenantEndpoint.cs [Migrated]
│   ├── GetTenantAsnRegistriesEndpoint.cs [Migrated]
│   └── RemoveAsnRegistryFromTenantEndpoint.cs [Migrated]
├── TenantAuthentication
│   ├── TenantLoginEndpoint.cs
│   └── TenantRefreshTokenEndpoint.cs
├── TenantAuthorization
│   ├── Action
│   │   └── GetTenantActionsEndpoint.cs [Migrated]
│   ├── Me
│   │   ├── GetTenantUserAccessiblePagesEndpoint.cs
│   │   ├── GetTenantUserPermissionsEndpoint.cs
│   │   └── GetTenantUserRolesMeEndpoint.cs
│   ├── Page
│   │   └── GetTenantPagesEndpoint.cs [Migrated]
│   ├── PageGroup
│   │   └── GetTenantPageGroupsEndpoint.cs [Migrated]
│   ├── Permission
│   │   └── GetTenantPermissionsEndpoint.cs [Migrated]
│   ├── Resource
│   │   └── GetTenantResourcesEndpoint.cs [Migrated]
│   ├── Role
│   │   ├── CreateTenantRoleEndpoint.cs [Migrated]
│   │   ├── DeleteTenantRoleEndpoint.cs [Migrated]
│   │   ├── GetTenantRoleByIdEndpoint.cs [Migrated]
│   │   ├── GetTenantRolesEndpoint.cs [Migrated]
│   │   ├── GetTenantRolesPaginatedEndpoint.cs [Migrated]
│   │   └── UpdateTenantRoleEndpoint.cs [Migrated]
│   ├── User
│   │   ├── CreateTenantUserEndpoint.cs [Migrated]
│   │   ├── DeleteTenantUserEndpoint.cs [Migrated]
│   │   ├── GetTenantUserByIdEndpoint.cs [Migrated]
│   │   ├── GetTenantUsersEndpoint.cs [Migrated]
│   │   └── UpdateTenantUserEndpoint.cs [Migrated]
│   └── UserRole
│   ├── CreateTenantUserRoleEndpoint.cs
│   ├── DeleteTenantUserRoleEndpoint.cs
│   ├── GetUserRolesEndpoint.cs
│   └── PutTenantUserRolesEndpoint.cs
├── TenantProtocols
│   ├── GetTenantProtocolByIdEndpoint.cs [Migrated]
│   └── GetTenantProtocolsEndpoint.cs [Migrated]
├── Tenants
│   ├── CreateTenantEndpoint.cs
│   ├── DeleteTenantEndpoint.cs
│   ├── GetTenantByIdEndpoint.cs
│   ├── GetTenantsPaginatedEndpoint.cs
│   └── UpdateTenantEndpoint.cs
├── TenantThreatEvents
│   ├── GetTenantThreatEventCategoryTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventComparativeTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventDashboardMetricsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventGeoHeatmapAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventMalwareFamilyTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventOverviewAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventProtocolDistributionAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventSourceCountriesTopAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventSummaryAnalyticsEndpoint.cs [Migrated]
│   ├── GetTenantThreatEventTimelineAnalyticsEndpoint.cs [Migrated]
│   └── GetTenantThreatEventTopAsnsAnalyticsEndpoint.cs [Migrated]
├── TenantUsers
│   ├── AssignRoleToTenantUserEndpoint.cs
│   ├── RemoveRoleFromTenantUserEndpoint.cs
│   ├── Roles
│   │   ├── AssignTenantUserRolesEndpoint.cs
│   │   ├── GetTenantUserRolesEndpoint.cs
│   │   └── RemoveTenantUserRoleEndpoint.cs
│   └── UpdateTenantUserEndpoint.cs
├── ThreatEvents
│   ├── CreateThreatEventEndpoint.cs
│   ├── DeleteThreatEventEndpoint.cs [Migrated]
│   ├── GetThreatEventByIdEndpoint.cs
│   ├── GetThreatEventCategoryTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventComparativeTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventDashboardMetricsEndpoint.cs [Migrated]
│   ├── GetThreatEventGeoHeatmapAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventMalwareFamilyTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventOverviewAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventProtocolDistributionAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventSourceCountriesTopAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventSummaryAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventTimelineAnalyticsEndpoint.cs [Migrated]
│   ├── GetThreatEventTopAsnsAnalyticsEndpoint.cs [Migrated]
│   └── UpdateThreatEventEndpoint.cs [Migrated]
└── Users
├── CreateUserEndpoint.cs
├── DeleteUserEndpoint.cs
├── GetUserByIdEndpoint.cs
├── GetUsersEndpoint.cs
└── UpdateUserEndpoint.cs
Legend:

- [Migrated]: Endpoint has been migrated to the new base type
- [Pending]: Endpoint still needs migration

Legend:

- [Migrated]: Endpoint has been migrated to the new base type
- [Pending]: Endpoint still needs migration

---

_Last updated: August 14, 2025_
