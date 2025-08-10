# Implementation Plan

- [x] 1. Create TimescaleDB lookup entities and configurations

  - Create AsnInfo, Country, Protocol, and MalwareFamily entity classes
  - Implement EF Core configurations for each lookup table with proper constraints
  - Add navigation properties and foreign key relationships
  - _Requirements: 2.1, 2.2_

- [x] 2. Update ThreatIntelligence entity for TimescaleDB schema

  - Modify ThreatIntelligence entity to use foreign keys instead of embedded data
  - Add IPAddress properties for source and destination addresses
  - Update navigation properties to reference lookup entities
  - Remove OptionalInformation embedded object in favor of direct properties
  - _Requirements: 2.1, 2.2_

- [x] 3. Create EF Core configurations for TimescaleDB integration

  - Implement ThreatIntelligenceConfiguration with PostgreSQL-specific mappings
  - Configure INET data type for IP address columns
  - Set up foreign key relationships with proper constraint names
  - Add database indexes for optimal query performance
  - _Requirements: 2.1, 2.2, 2.3_

- [x] 4. Create EF Core migration for TimescaleDB schema

  - Generate migration to create lookup tables (asn_info, countries, protocols, malware_families)
  - Generate migration to create threat_intelligence table with proper foreign keys
  - Add custom migration code to create TimescaleDB hypertable
  - Include index creation for time-series optimized queries
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 2.5_

- [x] 5. Update ApplicationDbContext for TimescaleDB entities

  - Add DbSet properties for new lookup entities
  - Configure PostgreSQL connection string handling
  - Add query filters for soft delete functionality on new entities
  - Update OnModelCreating to apply TimescaleDB configurations
  - _Requirements: 2.1, 2.2, 2.3_

- [x] 6. Create Go migration tool project structure

  - Initialize Go module with required dependencies (mongo-driver, pq, uuid)
  - Create main package structure with configuration management
  - Implement MongoDB connection and document reading functionality
  - Set up PostgreSQL connection with prepared statements for batch inserts
  - _Requirements: 3.1, 3.2, 3.3_

- [x] 7. Implement data transformation logic in Go migration tool ✅ **DONE**

  - Create struct mappings for MongoDB documents to PostgreSQL records
  - Implement lookup table population and ID resolution logic
  - Add IP address parsing and validation for INET columns
  - Handle data normalization for ASN, country, protocol, and malware family data
  - _Requirements: 3.1, 3.2, 3.3_

- [x] 8. Implement batch processing and error handling in migration tool ✅ **DONE**

  - Create worker pool pattern for parallel document processing
  - Implement batch insert functionality with configurable batch sizes
  - Add comprehensive error logging and recovery mechanisms
  - Create progress tracking with status updates and completion metrics
  - _Requirements: 3.3, 3.4, 3.5_

- [x] 9. Create updated repository implementation for TimescaleDB

  - Implement ThreatIntelligenceRepository extending base Repository<T>
  - Add TimescaleDB-specific query methods using time_bucket functions
  - Implement optimized time-range queries with proper includes
  - Add batch insert methods for high-performance data ingestion
  - _Requirements: 2.3, 2.4, 4.1, 4.2_

- [x] 10. Update query handlers to use new TimescaleDB repository

  - Modify GetThreatIntelligenceQueryHandler to use updated entity structure
  - Update GetThreatIntelligencePaginatedQueryHandler with TimescaleDB optimizations
  - Maintain existing API contracts and response formats
  - Add TimescaleDB-specific aggregation queries for performance
  - _Requirements: 4.1, 4.2, 4.3, 4.4_

- [ ] 11. Create data validation and comparison tools

  - Implement Go utility to compare MongoDB vs TimescaleDB record counts
  - Create sample data validation to verify transformation accuracy
  - Add performance benchmarking tools to measure query improvements
  - Implement data integrity checks for foreign key relationships
  - _Requirements: 5.1, 5.2, 5.4_

- [ ] 12. Add TimescaleDB continuous aggregates and policies

  - Create materialized views for hourly and daily threat summaries
  - Implement continuous aggregate policies for automatic refresh
  - Add compression policies for data older than specified intervals
  - Create retention policies for automatic data cleanup
  - _Requirements: 1.1, 1.5, 2.4_

- [ ] 13. Implement feature flag system for gradual rollout

  - Add configuration option to switch between MongoDB and TimescaleDB
  - Create service abstraction to support both data sources
  - Implement dual-write capability for testing and validation
  - Add monitoring and metrics collection for both systems
  - _Requirements: 5.3, 5.5_

- [ ] 14. Create comprehensive unit tests for new entities and configurations

  - Test EF Core entity mappings and relationships
  - Test TimescaleDB-specific query functionality
  - Test data transformation logic in migration tool
  - Test error handling and recovery mechanisms
  - _Requirements: 2.1, 2.2, 3.1, 3.2, 6.1, 6.2, 6.3_

- [ ] 15. Create integration tests for TimescaleDB functionality

  - Test end-to-end data flow from API to TimescaleDB
  - Test TimescaleDB hypertable functionality and partitioning
  - Test continuous aggregates and time-series specific features
  - Test performance improvements compared to MongoDB baseline
  - _Requirements: 1.1, 2.3, 2.4, 4.3, 4.4, 5.4_

- [ ] 16. Add monitoring and alerting for TimescaleDB operations
  - Implement database connection health checks
  - Add query performance monitoring and timeout handling
  - Create alerts for data inconsistencies and migration failures
  - Add metrics collection for query response times and throughput
  - _Requirements: 6.1, 6.2, 6.4, 6.5_
