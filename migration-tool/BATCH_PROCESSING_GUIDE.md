# Enhanced Batch Processing and Error Handling Guide

## Overview

The migration tool now includes comprehensive batch processing capabilities with advanced error handling, retry mechanisms, and detailed progress tracking. This implementation ensures robust data migration with maximum reliability and performance.

## Key Features Implemented

### 1. Enhanced Error Handling System

#### Error Classification

- **ValidationError**: Document structure or required field validation failures
- **TransformationError**: Data transformation and normalization failures
- **DatabaseError**: PostgreSQL connection, query, or constraint failures
- **NetworkError**: Network connectivity and timeout issues
- **UnknownError**: Unclassified errors for investigation

#### Error Logging

- **Structured Logging**: Each error includes type, document ID, timestamp, and retry status
- **Error Aggregation**: Real-time error counting by type for monitoring
- **Detailed Context**: Original error messages preserved for debugging

### 2. Advanced Retry Mechanisms

#### Batch-Level Retry

- **Configurable Retries**: Set maximum retry attempts per batch
- **Exponential Backoff**: Increasing delays between retry attempts
- **Timeout Protection**: Configurable batch processing timeouts
- **Context Cancellation**: Graceful handling of cancellation signals

#### Error-Specific Retry Logic

- **Retryable Error Detection**: Automatic identification of temporary failures
- **Network Error Handling**: Special handling for connection issues
- **Database Lock Handling**: Retry logic for deadlocks and lock timeouts
- **Non-Retryable Errors**: Skip retry for permanent failures (validation errors)

### 3. Worker Pool Pattern

#### Concurrent Processing

- **Configurable Workers**: Set optimal worker count for your system
- **Channel-Based Communication**: Efficient batch distribution
- **Graceful Shutdown**: Proper cleanup on interruption signals
- **Resource Management**: Controlled memory and connection usage

#### Load Balancing

- **Dynamic Batch Distribution**: Workers pull batches as they become available
- **Buffer Management**: Configurable channel buffer sizes
- **Backpressure Handling**: Automatic flow control when workers are busy

### 4. Progress Tracking and Monitoring

#### Real-Time Metrics

- **Processing Rate**: Documents per second calculation
- **Progress Percentage**: Completion status with ETA estimation
- **Error Breakdown**: Live error counts by category
- **Performance Metrics**: Batch processing times and throughput

#### Configurable Reporting

- **Report Intervals**: Customizable progress report frequency
- **Detailed Summaries**: Comprehensive migration completion reports
- **Health Checks**: Connection and system status validation

## Configuration Options

### Environment Variables

```bash
# Batch Processing Configuration
MIGRATION_BATCH_SIZE=1000                    # Documents per batch
MIGRATION_WORKER_COUNT=10                    # Concurrent workers
MIGRATION_BUFFER_SIZE=100                    # Channel buffer size
MIGRATION_CONNECTION_POOL_SIZE=20            # Database connections

# Error Handling Configuration
MIGRATION_MAX_RETRIES=3                      # Maximum retry attempts
MIGRATION_RETRY_DELAY_SECONDS=5              # Base retry delay
MIGRATION_BATCH_TIMEOUT_SECONDS=300          # Batch processing timeout
MIGRATION_PROGRESS_REPORT_INTERVAL=30        # Progress report frequency
```

### Optimal Configuration Guidelines

#### Small Datasets (< 100K documents)

```bash
MIGRATION_BATCH_SIZE=500
MIGRATION_WORKER_COUNT=5
MIGRATION_BUFFER_SIZE=50
```

#### Medium Datasets (100K - 1M documents)

```bash
MIGRATION_BATCH_SIZE=1000
MIGRATION_WORKER_COUNT=10
MIGRATION_BUFFER_SIZE=100
```

#### Large Datasets (> 1M documents)

```bash
MIGRATION_BATCH_SIZE=2000
MIGRATION_WORKER_COUNT=20
MIGRATION_BUFFER_SIZE=200
```

## Usage Examples

### Basic Migration

```bash
# Set required environment variables
export POSTGRES_PASSWORD="your_password"
export MONGO_URI="mongodb://localhost:27017"

# Run migration
./migration-tool
```

### Health Check

```bash
# Verify connections and system status
./migration-tool health
```

### Test Transformation Logic

```bash
# Test data transformation without database operations
./migration-tool test
```

## Error Handling Scenarios

### 1. Network Connectivity Issues

- **Detection**: Connection refused, timeouts, network unreachable
- **Response**: Automatic retry with exponential backoff
- **Recovery**: Continues processing once connection restored

### 2. Database Constraint Violations

- **Detection**: Foreign key violations, unique constraints
- **Response**: Log error and skip document (non-retryable)
- **Recovery**: Continue with remaining documents

### 3. Temporary Database Overload

- **Detection**: Too many connections, lock timeouts
- **Response**: Retry with increased delays
- **Recovery**: Automatic recovery when load decreases

### 4. Data Quality Issues

- **Detection**: Invalid IP addresses, malformed data
- **Response**: Log validation error and skip document
- **Recovery**: Continue processing valid documents

## Performance Optimization

### 1. Batch Size Tuning

- **Small Batches**: Better error isolation, higher overhead
- **Large Batches**: Better throughput, harder error recovery
- **Recommendation**: Start with 1000, adjust based on performance

### 2. Worker Count Optimization

- **Too Few Workers**: Underutilized resources
- **Too Many Workers**: Resource contention, diminishing returns
- **Recommendation**: 2x CPU cores, monitor resource usage

### 3. Connection Pool Sizing

- **Insufficient Connections**: Worker blocking, reduced throughput
- **Excessive Connections**: Database resource exhaustion
- **Recommendation**: 2x worker count, respect database limits

## Monitoring and Troubleshooting

### Progress Monitoring

```
Progress: 45.2% (452000/1000000), Rate: 1250.3 docs/sec, Total Errors: 23, ETA: 7m15s
Error Breakdown - Validation: 5, Transformation: 8, Database: 7, Network: 3, Unknown: 0
```

### Migration Summary

```
================================================================================
MIGRATION SUMMARY
================================================================================
Total Documents: 1000000
Successfully Processed: 999977 (99.998%)
Failed Documents: 23 (0.002%)
Total Elapsed Time: 13m42s
Average Processing Rate: 1214.5 docs/sec

ERROR BREAKDOWN:
  Validation Errors: 5
  Transformation Errors: 8
  Database Errors: 7
  Network Errors: 3
  Unknown Errors: 0

PERFORMANCE METRICS:
  Worker Count: 10
  Batch Size: 1000
  Buffer Size: 100
  Max Retries: 3

MIGRATION STATUS:
  ⚠️  COMPLETED WITH ERRORS - 23 documents failed to migrate
     Check error logs above for detailed error information
================================================================================
```

### Common Issues and Solutions

#### High Error Rates

1. **Check Data Quality**: Review validation and transformation errors
2. **Verify Connectivity**: Ensure stable database connections
3. **Adjust Batch Size**: Smaller batches for better error isolation

#### Poor Performance

1. **Increase Workers**: Add more concurrent workers
2. **Optimize Batch Size**: Find optimal batch size for your data
3. **Check Resources**: Monitor CPU, memory, and network usage

#### Connection Issues

1. **Verify Credentials**: Ensure correct database credentials
2. **Check Network**: Verify network connectivity and firewall rules
3. **Increase Timeouts**: Adjust timeout values for slow networks

## Recovery and Resumption

### Failed Migration Recovery

- **Error Logging**: All errors logged with document IDs for investigation
- **Partial Success**: Successfully migrated documents remain in target database
- **Retry Strategy**: Re-run migration to process only failed documents

### Data Integrity Verification

- **Document Counts**: Compare source and target document counts
- **Sample Validation**: Verify transformation accuracy on sample data
- **Foreign Key Integrity**: Ensure all lookup table relationships are valid

## Best Practices

### 1. Pre-Migration Preparation

- Run health checks to verify system readiness
- Test transformation logic with sample data
- Configure appropriate batch sizes and worker counts
- Ensure sufficient database connection limits

### 2. During Migration

- Monitor progress reports for performance issues
- Watch error rates and investigate high error counts
- Ensure sufficient system resources (CPU, memory, network)
- Have database monitoring in place

### 3. Post-Migration Validation

- Compare document counts between source and target
- Validate sample data transformation accuracy
- Verify foreign key relationships and data integrity
- Run application tests against migrated data

### 4. Production Deployment

- Use feature flags for gradual rollout
- Implement dual-write capability for validation
- Monitor application performance with new data source
- Have rollback plan ready if issues arise

## Conclusion

The enhanced batch processing and error handling system provides enterprise-grade reliability for large-scale data migrations. The comprehensive error handling, retry mechanisms, and monitoring capabilities ensure successful migration even in challenging environments with network instability or data quality issues.

## UTF-8 Data Sanitization

### Problem Addressed

Real-world MongoDB data often contains problematic characters that cause PostgreSQL UTF-8 encoding errors:

- Null bytes (0x00) in string fields
- Invalid control characters
- Malformed UTF-8 sequences

### Solution Implemented

- **Automatic Sanitization**: All string fields are sanitized before database insertion
- **Null Byte Removal**: Removes 0x00 bytes that cause "invalid byte sequence for encoding UTF8" errors
- **Control Character Filtering**: Removes problematic control characters while preserving valid ones (tab, newline, CR)
- **UTF-8 Validation**: Ensures all strings are valid UTF-8 before database operations

### Testing Results

```
✅ String with null bytes: "IDNIC-PAAS-AS-ID\x00 PT. Awan Kilat\x00 Semesta, ID" → "IDNIC-PAAS-AS-ID PT. Awan Kilat Semesta, ID"
✅ Control characters: "Test\x01\x02\x03String\x1f" → "TestString"
✅ Valid control chars preserved: "Line1\tTabbed\nLine2\rCarriageReturn" → unchanged
✅ Normal strings: unchanged
✅ Empty strings: handled correctly
```

The system is designed to be resilient, performant, and observable, providing detailed insights into the migration process and enabling quick resolution of any issues that arise. The UTF-8 sanitization ensures compatibility with real-world data quality issues commonly found in production MongoDB collections.
