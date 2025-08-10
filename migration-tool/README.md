# TimescaleDB Migration Tool

This Go application migrates threat intelligence data from MongoDB to TimescaleDB (PostgreSQL extension).

## Features

- **Parallel Processing**: Uses configurable worker pools for efficient data processing
- **Batch Operations**: Processes documents in batches for optimal performance
- **Progress Tracking**: Real-time progress reporting with ETA calculations
- **Error Handling**: Comprehensive error logging and recovery mechanisms
- **Graceful Shutdown**: Handles interrupt signals for clean termination
- **Data Normalization**: Transforms MongoDB documents to normalized PostgreSQL records
- **Lookup Caching**: Caches lookup table data for improved performance

## Prerequisites

- Go 1.21 or later
- MongoDB instance with threat intelligence data
- PostgreSQL with TimescaleDB extension
- Required database tables created (see migration files in the main project)

## Configuration

The tool is configured via environment variables:

### MongoDB Configuration

- `MONGO_URI`: MongoDB connection URI (default: `mongodb://localhost:27017`)
- `MONGO_DATABASE`: MongoDB database name (default: `threat_intelligence`)
- `MONGO_COLLECTION`: MongoDB collection name (default: `threats`)

### PostgreSQL Configuration

- `POSTGRES_HOST`: PostgreSQL host (default: `localhost`)
- `POSTGRES_PORT`: PostgreSQL port (default: `5432`)
- `POSTGRES_DATABASE`: PostgreSQL database name (default: `threat_intelligence`)
- `POSTGRES_USERNAME`: PostgreSQL username (default: `postgres`)
- `POSTGRES_PASSWORD`: PostgreSQL password (required)
- `POSTGRES_SSLMODE`: SSL mode (default: `disable`)

### Migration Configuration

- `MIGRATION_BATCH_SIZE`: Number of documents per batch (default: `1000`)
- `MIGRATION_WORKER_COUNT`: Number of worker goroutines (default: `10`)
- `MIGRATION_BUFFER_SIZE`: Channel buffer size (default: `100`)
- `MIGRATION_CONNECTION_POOL_SIZE`: PostgreSQL connection pool size (default: `20`)

## Usage

1. Set the required environment variables:

```bash
export POSTGRES_PASSWORD="your_password"
export MONGO_URI="mongodb://your-mongo-host:27017"
export POSTGRES_HOST="your-postgres-host"
```

2. Build the application:

```bash
go build -o migration-tool .
```

3. Run the migration:

```bash
./migration-tool
```

## Data Transformation

The tool transforms MongoDB documents to normalized PostgreSQL records:

- **Document ID**: MongoDB ObjectID → PostgreSQL UUID
- **Timestamps**: Direct mapping with timezone handling
- **IP Addresses**: String → PostgreSQL INET type
- **Lookup Data**: Normalized into separate tables (ASN, countries, protocols, malware families)
- **Optional Fields**: Handled with proper NULL values

## Performance Tuning

For optimal performance, consider adjusting these parameters based on your environment:

- **Batch Size**: Larger batches reduce overhead but use more memory
- **Worker Count**: Should match your CPU cores and database capacity
- **Connection Pool**: Should accommodate worker count and database limits
- **Buffer Size**: Affects memory usage and throughput

## Monitoring

The tool provides real-time progress updates including:

- Percentage completion
- Processing rate (documents/second)
- Error count
- Estimated time to completion

## Error Handling

- **Validation Errors**: Documents failing validation are logged and skipped
- **Transformation Errors**: Documents that can't be transformed are logged and skipped
- **Database Errors**: Connection issues and constraint violations are handled gracefully
- **Graceful Shutdown**: SIGINT/SIGTERM signals trigger clean shutdown

## Logging

All operations are logged with timestamps and severity levels:

- Progress updates every 30 seconds
- Error details for failed documents
- Connection status and statistics
- Final migration summary

## Example Output

```
2024/08/08 10:00:00 Connected to MongoDB: threat_intelligence/threats
2024/08/08 10:00:00 Connected to PostgreSQL: localhost:5432/threat_intelligence
2024/08/08 10:00:00 Loaded lookup caches: 1500 ASNs, 250 countries, 10 protocols, 50 malware families
2024/08/08 10:00:00 Starting migration of 1000000 documents from MongoDB
2024/08/08 10:00:30 Progress: 5.2% (52000/1000000), Rate: 1733.3 docs/sec, Errors: 12, ETA: 9m15s
2024/08/08 10:15:45 Migration completed: 1000000/1000000 documents processed, 45 errors, elapsed: 15m45s
```
