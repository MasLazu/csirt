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
			BatchSize:              getEnvIntOrDefault("MIGRATION_BATCH_SIZE", 10000),
			WorkerCount:            getEnvIntOrDefault("MIGRATION_WORKER_COUNT", 10),
			BufferSize:             getEnvIntOrDefault("MIGRATION_BUFFER_SIZE", 100),
			ConnectionPoolSize:     getEnvIntOrDefault("MIGRATION_CONNECTION_POOL_SIZE", 20),
			MaxRetries:             getEnvIntOrDefault("MIGRATION_MAX_RETRIES", 3),
			RetryDelaySeconds:      getEnvIntOrDefault("MIGRATION_RETRY_DELAY_SECONDS", 5),
			BatchTimeoutSeconds:    getEnvIntOrDefault("MIGRATION_BATCH_TIMEOUT_SECONDS", 300),
			ProgressReportInterval: getEnvIntOrDefault("MIGRATION_PROGRESS_REPORT_INTERVAL", 30),
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
