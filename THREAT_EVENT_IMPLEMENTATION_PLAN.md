# ThreatEvent Management Implementation Plan

## 🎯 Overview

Implementing comprehensive ThreatEvent management system for a **TimescaleDB hypertable** with over 25 million records, optimized for time-series cybersecurity data.

## 📊 Database Analysis Results

### ✅ **Hypertable Verification**

- **Table**: `ThreatEvents` exists as a **TimescaleDB hypertable**
- **Primary Dimension**: `Timestamp` (timestamp with time zone)
- **Chunks**: 60 active chunks
- **Records**: **25,121,436 threat events** (production data!)
- **Compression**: Not enabled (potential optimization)

### 📋 **Schema Structure**

```sql
ThreatEvents (Hypertable):
├── Id (uuid, NOT NULL) - Part of composite PK
├── Timestamp (timestamptz, NOT NULL) - Hypertable dimension + Part of composite PK
├── AsnRegistryId (uuid, NOT NULL) - FK to AsnRegistries
├── SourceAddress (inet, NOT NULL) - IP address
├── SourceCountryId (uuid, nullable) - FK to Countries
├── DestinationAddress (inet, nullable) - IP address
├── DestinationCountryId (uuid, nullable) - FK to Countries
├── SourcePort (integer, nullable)
├── DestinationPort (integer, nullable)
├── ProtocolId (uuid, nullable) - FK to Protocols
├── Category (varchar(50), NOT NULL) - Threat category
├── MalwareFamilyId (uuid, nullable) - FK to MalwareFamilies
├── CreatedAt (timestamptz, NOT NULL)
├── UpdatedAt (timestamptz, nullable)
└── DeletedAt (timestamptz, nullable) - Soft delete
```

### 🔍 **Optimized Indexes**

- **Composite PK**: (Id, Timestamp) - Required for hypertable
- **Time Index**: Timestamp DESC - Time-series queries
- **Foreign Keys**: AsnRegistryId, SourceCountryId, DestinationCountryId, etc.
- **Query Indexes**: SourceAddress, DestinationAddress, Category, etc.

## 🏗️ Proposed Implementation: **Time-Series Cybersecurity Analytics**

Given the massive scale (25M+ records) and hypertable structure, this requires **specialized time-series management**:

### 📋 **Implementation Approach: High-Performance Time-Series Management**

**Core Endpoints (Admin + Tenant Users):**

- `GET /api/v1/threat-events` - List threat events (time-range filtered, highly optimized)
- `GET /api/v1/threat-events/{id}` - Get threat event by ID
- `POST /api/v1/threat-events` - Create threat event (real-time ingestion)
- `PUT /api/v1/threat-events/{id}` - Update threat event
- `DELETE /api/v1/threat-events/{id}` - Delete threat event (soft delete)

**Time-Series Analytics Endpoints:**

- `GET /api/v1/threat-events/analytics/timeline` - Time-series aggregation
- `GET /api/v1/threat-events/analytics/summary` - Summary statistics
- `GET /api/v1/threat-events/analytics/top-sources` - Top source IPs/countries
- `GET /api/v1/threat-events/analytics/malware-trends` - Malware family trends

**Tenant-Scoped Access:**

- `GET /api/v1/tenants/{tenantId}/threat-events` - Tenant's ASN-scoped threat events
- `GET /api/v1/tenants/{tenantId}/threat-events/analytics/*` - Tenant analytics

## 🔐 Proposed Permissions

### Admin Permissions:

- `CREATE:THREAT_EVENT` - Create new threat events
- `READ:THREAT_EVENT` - View all threat events
- `UPDATE:THREAT_EVENT` - Modify threat events
- `DELETE:THREAT_EVENT` - Remove threat events
- `ANALYTICS:THREAT_EVENT` - Access analytics and reporting

### Tenant User Permissions:

- `READ:TENANT_THREAT_EVENT` - View tenant's ASN-scoped threat events
- `ANALYTICS:TENANT_THREAT_EVENT` - Tenant-scoped analytics

## ⚡ **Time-Series Optimization Features**

### 🚀 **Query Optimization**

1. **Time-Range Filtering**: All queries MUST include time range (last 24h, 7d, 30d)
2. **Chunk Exclusion**: Leverage TimescaleDB's automatic chunk exclusion
3. **Index Usage**: Optimize for time-first, then other filters
4. **Aggregation**: Use TimescaleDB's time-bucket functions
5. **Pagination**: Time-based cursor pagination instead of offset

### 📊 **Specialized Query Patterns**

```sql
-- Time-bucket aggregations (built into queries)
SELECT time_bucket('1 hour', "Timestamp") as bucket,
       COUNT(*) as threat_count
FROM "ThreatEvents"
WHERE "Timestamp" >= NOW() - INTERVAL '24 hours'
GROUP BY bucket ORDER BY bucket;

-- ASN-scoped queries for tenants
SELECT * FROM "ThreatEvents" te
JOIN "TenantAsnRegistries" tar ON te."AsnRegistryId" = tar."AsnRegistryId"
WHERE tar."TenantId" = $1
  AND te."Timestamp" >= $2
  AND te."Timestamp" <= $3;
```

### 🎯 **Performance Considerations**

- **NO full table scans** - Always include time range
- **Efficient joins** with reference data (Countries, MalwareFamilies, etc.)
- **Streaming responses** for large result sets
- **Caching** for analytics queries
- **Rate limiting** for expensive operations

## 📁 Required File Structure

### Application Layer

```
/src/MeUi.Application/Features/ThreatEvents/
├── Commands/
│   ├── CreateThreatEvent/ (Command, Handler, Validator)
│   ├── UpdateThreatEvent/ (Command, Handler, Validator)
│   └── DeleteThreatEvent/ (Command, Handler, Validator)
├── Queries/
│   ├── GetThreatEventsPaginated/ (Time-range optimized)
│   ├── GetThreatEvent/ (Single event by ID)
│   ├── GetThreatEventAnalytics/ (Time-series analytics)
│   └── GetThreatEventTimeline/ (Time-bucket aggregations)
└── Models/
    ├── ThreatEventTimelineDto.cs
    ├── ThreatEventAnalyticsDto.cs
    └── ThreatEventFilters.cs
```

### API Layer

```
/src/MeUi.Api/Endpoints/ThreatEvents/
├── CreateThreatEventEndpoint.cs
├── GetThreatEventsEndpoint.cs (Time-range required)
├── GetThreatEventByIdEndpoint.cs
├── UpdateThreatEventEndpoint.cs
├── DeleteThreatEventEndpoint.cs
├── GetThreatEventAnalyticsEndpoint.cs
└── GetThreatEventTimelineEndpoint.cs

/src/MeUi.Api/Endpoints/TenantThreatEvents/
├── GetTenantThreatEventsEndpoint.cs (ASN-scoped)
└── GetTenantThreatEventAnalyticsEndpoint.cs
```

## 📋 Validation Rules

### ThreatEvent Validation:

- `AsnRegistryId`: Required, must exist
- `SourceAddress`: Required, valid IP address
- `Timestamp`: Required, cannot be future date
- `Category`: Required, 2-50 characters
- `SourcePort/DestinationPort`: Valid port range (1-65535)
- **Time Range**: Queries must specify reasonable time ranges (max 90 days)

### Performance Validation:

- **Query Time Range**: Maximum 90 days for single query
- **Result Limit**: Maximum 10,000 records per query
- **Analytics Period**: Configurable aggregation periods
- **Rate Limiting**: Max 100 requests/minute per user

## 🎯 Business Logic

### **Time-Series Query Pattern**

All ThreatEvent queries follow this pattern:

1. **Validate time range** (required, max 90 days)
2. **Apply time filter first** (chunk exclusion)
3. **Add secondary filters** (ASN, IP, Country, etc.)
4. **Use appropriate indexes**
5. **Apply tenant scoping** if needed
6. **Return paginated results** with time-based cursors

### **Tenant Isolation**

Tenant users can only see threat events for their assigned ASN registries:

```sql
WHERE te.AsnRegistryId IN (
  SELECT tar.AsnRegistryId
  FROM TenantAsnRegistries tar
  WHERE tar.TenantId = @tenantId
)
```

### **Analytics Optimization**

- **Pre-aggregated views** for common analytics
- **Time-bucket functions** for timeline data
- **Materialized views** for expensive calculations
- **Background jobs** for heavy analytics

## ⚠️ **Critical Performance Notes**

### 🚨 **NEVER Allow These Queries**

- Full table scans without time range
- Queries without proper indexes
- Offset-based pagination on large datasets
- Queries spanning more than 90 days without aggregation

### ✅ **Always Enforce**

- Time range filtering (last 24h, 7d, 30d default)
- Efficient indexing strategy
- Tenant-scoped access control
- Result set size limits
- Query timeout limits

## 🧪 Special Considerations for 25M+ Records

### **Streaming Responses**

- Use `IAsyncEnumerable<T>` for large result sets
- Implement streaming JSON responses
- Time-based cursor pagination

### **Database Optimization**

- **Enable compression** on older chunks
- **Data retention policies** for old threat events
- **Continuous aggregates** for real-time analytics
- **Background jobs** for maintenance

### **Monitoring & Alerting**

- Query performance monitoring
- Slow query detection
- Resource usage tracking
- Rate limiting enforcement

Would you like me to proceed with implementing this **high-performance time-series ThreatEvent management system** optimized for the 25M+ record hypertable?
