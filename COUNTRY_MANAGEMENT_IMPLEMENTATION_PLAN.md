# Country Management Implementation Plan

## 🎯 Overview

Implementing comprehensive Country management system following the same patterns established for ASN Registry management.

## 📊 Current State Analysis

- ✅ `Country` entity exists (`Code`, `Name`, relationships to `ThreatEvent`)
- ✅ `CountryDto` exists and matches entity structure
- ❌ No Country management endpoints implemented
- ❌ No Country CRUD operations
- ❌ No tenant-country assignment (unlike ASN registries)

## 🏗️ Proposed Implementation

### Option 1: Global Country Management Only

Countries are treated as **global reference data** that all tenants can access (like master country list).

**Endpoints:**

- `GET /api/v1/countries` - List all countries (paginated, searchable)
- `GET /api/v1/countries/{id}` - Get country by ID
- `POST /api/v1/countries` - Create country (admin only)
- `PUT /api/v1/countries/{id}` - Update country (admin only)
- `DELETE /api/v1/countries/{id}` - Delete country (admin only)

### Option 2: Global + Tenant Assignment (Recommended)

Countries can be assigned to specific tenants for data filtering and security isolation.

**Global Management (Admin Only):**

- `GET /api/v1/countries` - List all countries
- `GET /api/v1/countries/{id}` - Get country by ID
- `POST /api/v1/countries` - Create country
- `PUT /api/v1/countries/{id}` - Update country
- `DELETE /api/v1/countries/{id}` - Delete country

**Tenant Assignment (Admin Only):**

- `POST /api/v1/tenants/{tenantId}/countries` - Assign countries to tenant
- `POST /api/v1/tenants/{tenantId}/countries/bulk` - Bulk assign countries
- `DELETE /api/v1/tenants/{tenantId}/countries/{countryId}` - Remove country from tenant

**Tenant Access (Tenant Users Read-Only):**

- `GET /api/v1/tenants/{tenantId}/countries` - List tenant's assigned countries

## 🔐 Proposed Permissions

### Admin Permissions:

- `CREATE:COUNTRY` - Create new countries
- `READ:COUNTRY` - View all countries
- `UPDATE:COUNTRY` - Modify countries
- `DELETE:COUNTRY` - Remove countries

### Tenant User Permissions:

- `READ:COUNTRY` - View all countries (global reference data)

## 📁 Required File Structure

### Application Layer

```
/src/MeUi.Application/Features/Countries/
├── Commands/
│   ├── CreateCountry/ (Command, Handler, Validator)
│   ├── UpdateCountry/ (Command, Handler, Validator)
│   └── DeleteCountry/ (Command, Handler, Validator)
└── Queries/
    ├── GetCountriesPaginated/ (Query, Handler, Validator)
    └── GetCountry/ (Query, Handler, Validator)
```

### API Layer

```
/src/MeUi.Api/Endpoints/
└── Countries/ (5 endpoints with IPermissionProvider)
```

## 💡 Recommendation: Option 1 (Global Country Management Only)

I recommend **Option 1** for the following reasons:

1. **Domain Design**: No TenantCountry entity exists, indicating countries are global reference data
2. **Standard Practice**: Countries are typically global master data (like ISO country codes)
3. **Simplicity**: Easier to manage and maintain as reference data
4. **Universal Access**: All tenants can reference any country for threat events
5. **Consistency**: Follows common patterns for reference/lookup data

## 🚀 Implementation Steps

1. **Implement Country CRUD commands/queries**
2. **Create API endpoints with proper permissions**
3. **Add comprehensive validation and error handling**
4. **Add Swagger documentation**
5. **Testing and verification**

## 📋 Validation Rules

### Country Validation:

- `Code`: Required, 2-3 characters, uppercase (ISO country codes)
- `Name`: Required, 2-100 characters
- `Code`: Must be unique globally
- `Name`: Must be unique globally

### Assignment Validation (Option 2):

- Tenant must exist
- Countries must exist
- No duplicate assignments
- Bulk operations: max 100 countries per request

Would you like me to proceed with implementing **Option 1 (Global Country Management)** following this plan?
