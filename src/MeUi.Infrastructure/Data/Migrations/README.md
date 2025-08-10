# Database Migrations

This directory contains Entity Framework Core migrations for the MeUi application.

## Migration History

### 20250806085949_InitialUnifiedMigration

Initial migration containing user authentication and authorization entities.

### 20250806101453_AddPageEntitiesFixed

Added page and page group entities for UI navigation.

### 20250807063433_FixPagePermissionsTable

Fixed page permissions table relationships.

### 20250808000000_AddTimescaleDBSchema

**TimescaleDB Migration for Threat Intelligence Data**

This migration introduces the TimescaleDB schema for storing threat intelligence data with the following components:

#### Lookup Tables

- `asn_info` - ASN information lookup table
- `countries` - Country codes and names
- `protocols` - Network protocols (TCP, UDP, etc.)
- `malware_families` - Malware family classifications

#### Main Table

- `threat_intelligence` - Main hypertable for time-series threat data

#### Key Features

1. **TimescaleDB Hypertable**: The `threat_intelligence` table is converted to a hypertable partitioned by timestamp with 1-day intervals
2. **Optimized Indexes**: Multiple indexes for time-series queries, IP address lookups, and composite queries
3. **Continuous Aggregates**: Pre-computed hourly and daily summaries for performance
4. **Compression Policy**: Automatic compression for data older than 7 days
5. **Retention Policy**: Automatic cleanup of data older than 2 years
6. **Foreign Key Relationships**: Normalized data structure with proper referential integrity

#### Performance Optimizations

- Hash indexes for IP address lookups
- Composite indexes for common query patterns
- Time-series specific indexes with descending timestamp ordering
- Filtered indexes for optional fields

#### TimescaleDB-Specific Features

- Hypertable partitioning by timestamp
- Continuous aggregates for hourly and daily summaries
- Compression policies for storage optimization
- Retention policies for data lifecycle management

## Running Migrations

To apply migrations to your database:

```bash
dotnet ef database update --project src/MeUi.Infrastructure --startup-project src/MeUi.Api
```

To generate a new migration:

```bash
dotnet ef migrations add MigrationName --project src/MeUi.Infrastructure --startup-project src/MeUi.Api
```

## TimescaleDB Requirements

The TimescaleDB migration requires:

1. PostgreSQL database with TimescaleDB extension installed
2. Proper permissions to create extensions and hypertables
3. TimescaleDB version 2.0 or higher

## Notes

- The TimescaleDB migration includes custom SQL for hypertable creation and policies
- Ensure TimescaleDB extension is available before running the migration
- The migration is designed to be reversible, but rolling back will lose TimescaleDB-specific optimizations
