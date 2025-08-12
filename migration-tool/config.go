package main

import (
	"fmt"
	"log"
	"os"
	"strconv"

	"github.com/joho/godotenv"
)

// Config holds all configuration for the migration tool
type Config struct {
	MongoDB    MongoConfig
	PostgreSQL PostgreSQLConfig
	Migration  MigrationConfig
}

// MongoConfig holds MongoDB connection configuration
type MongoConfig struct {
	URI        string
	Database   string
	Collection string
}

// PostgreSQLConfig holds PostgreSQL connection configuration
type PostgreSQLConfig struct {
	Host     string
	Port     int
	Database string
	Username string
	Password string
	SSLMode  string
}

// MigrationConfig holds migration-specific configuration
type MigrationConfig struct {
	BatchSize              int
	WorkerCount            int
	BufferSize             int
	ConnectionPoolSize     int
	MaxRetries             int
	RetryDelaySeconds      int
	BatchTimeoutSeconds    int
	ProgressReportInterval int
	// Performance / ingestion tuning
	UseCopy                  bool   // enable high-throughput COPY ingestion
	CopyThreshold            int    // minimum batch size before switching to COPY
	CopyTempTable            string // optional staging table name
	DisableIndexesDuringLoad bool   // if true, non-critical indexes will be disabled (future use)
	// Adaptive COPY tuning
	AdaptiveCopy         bool // enable adaptive COPY based on system load
	CopyTargetRowsPerSec int  // target rows per second for adaptive COPY
	CopyMinThreshold     int  // minimum batch size for adaptive COPY
	CopyMaxThreshold     int  // maximum batch size for adaptive COPY
	DryRun               bool // do transformation & validation only (no DB writes)
}

// LoadConfig loads configuration from environment variables with defaults
func LoadConfig() (*Config, error) {
	// Load .env file if it exists
	if err := godotenv.Load(); err != nil {
		log.Printf("Warning: Could not load .env file: %v", err)
	} else {
		log.Println("Loaded configuration from .env file")
		// log.Printf("DEBUG: POSTGRES_HOST from env: %s", os.Getenv("POSTGRES_HOST"))
	}

	config := &Config{
		MongoDB: MongoConfig{
			URI:        getEnvOrDefault("MONGO_URI", "mongodb://localhost:27017"),
			Database:   getEnvOrDefault("MONGO_DATABASE", "csrit"),
			Collection: getEnvOrDefault("MONGO_COLLECTION", "ThreatIntelligence"),
		},
		PostgreSQL: PostgreSQLConfig{
			Host:     getEnvOrDefault("POSTGRES_HOST", "localhost"),
			Port:     getEnvIntOrDefault("POSTGRES_PORT", 5432),
			Database: getEnvOrDefault("POSTGRES_DATABASE", "csrit"),
			Username: getEnvOrDefault("POSTGRES_USERNAME", "postgres"),
			Password: getEnvOrDefault("POSTGRES_PASSWORD", ""),
			SSLMode:  getEnvOrDefault("POSTGRES_SSLMODE", "disable"),
		},
		Migration: MigrationConfig{
			BatchSize:                getEnvIntOrDefault("MIGRATION_BATCH_SIZE", 10000),
			WorkerCount:              getEnvIntOrDefault("MIGRATION_WORKER_COUNT", 10),
			BufferSize:               getEnvIntOrDefault("MIGRATION_BUFFER_SIZE", 100),
			ConnectionPoolSize:       getEnvIntOrDefault("MIGRATION_CONNECTION_POOL_SIZE", 20),
			MaxRetries:               getEnvIntOrDefault("MIGRATION_MAX_RETRIES", 3),
			RetryDelaySeconds:        getEnvIntOrDefault("MIGRATION_RETRY_DELAY_SECONDS", 5),
			BatchTimeoutSeconds:      getEnvIntOrDefault("MIGRATION_BATCH_TIMEOUT_SECONDS", 300),
			ProgressReportInterval:   getEnvIntOrDefault("MIGRATION_PROGRESS_REPORT_INTERVAL", 30),
			UseCopy:                  getEnvBoolOrDefault("MIGRATION_USE_COPY", true),
			CopyThreshold:            getEnvIntOrDefault("MIGRATION_COPY_THRESHOLD", 2000),
			CopyTempTable:            getEnvOrDefault("MIGRATION_COPY_TEMP_TABLE", ""),
			DisableIndexesDuringLoad: getEnvBoolOrDefault("MIGRATION_DISABLE_INDEXES", false),
			AdaptiveCopy:             getEnvBoolOrDefault("MIGRATION_COPY_ADAPTIVE", true),
			CopyTargetRowsPerSec:     getEnvIntOrDefault("MIGRATION_COPY_TARGET_RPS", 25000),
			CopyMinThreshold:         getEnvIntOrDefault("MIGRATION_COPY_MIN_THRESHOLD", 500),
			CopyMaxThreshold:         getEnvIntOrDefault("MIGRATION_COPY_MAX_THRESHOLD", 50000),
			DryRun:                   getEnvBoolOrDefault("MIGRATION_DRY_RUN", false),
		},
	}

	// Validate required configuration
	if config.PostgreSQL.Password == "" {
		return nil, fmt.Errorf("POSTGRES_PASSWORD environment variable is required")
	}

	return config, nil
}

// PostgreSQLConnectionString returns a PostgreSQL connection string
func (c *PostgreSQLConfig) ConnectionString() string {
	return fmt.Sprintf("host=%s port=%d user=%s password=%s dbname=%s sslmode=%s",
		c.Host, c.Port, c.Username, c.Password, c.Database, c.SSLMode)
}

// getEnvOrDefault returns environment variable value or default if not set
func getEnvOrDefault(key, defaultValue string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}
	return defaultValue
}

// getEnvIntOrDefault returns environment variable as int or default if not set/invalid
func getEnvIntOrDefault(key string, defaultValue int) int {
	if value := os.Getenv(key); value != "" {
		if intValue, err := strconv.Atoi(value); err == nil {
			return intValue
		}
	}
	return defaultValue
}

// getEnvBoolOrDefault returns environment variable as bool or default if not set/invalid
func getEnvBoolOrDefault(key string, defaultValue bool) bool {
	if value := os.Getenv(key); value != "" {
		if v, err := strconv.ParseBool(value); err == nil {
			return v
		}
	}
	return defaultValue
}
