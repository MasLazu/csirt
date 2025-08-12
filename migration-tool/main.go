package main

import (
	"context"
	"fmt"
	"log"
	"os"
	"os/signal"
	"strings"
	"sync"
	"syscall"
	"time"
)

// MigrationTool orchestrates the migration process
type MigrationTool struct {
	config         *Config
	mongoClient    *MongoDBClient
	postgresClient *PostgreSQLClient

	// Enhanced error handling and batch processing
	batchProcessor *BatchProcessor
	errorLogger    *ErrorLogger

	// Progress tracking
	totalDocuments     int64
	processedDocuments int64
	errorCount         int64
	startTime          time.Time

	// Synchronization
	mu sync.RWMutex
}

// MigrationResult represents the result of processing a batch
type MigrationResult struct {
	ProcessedCount int
	ErrorCount     int
	Errors         []error
	BatchID        int
	ProcessingTime time.Duration
	RetryCount     int
}

// ErrorType represents different types of errors that can occur during migration
type ErrorType int

const (
	ValidationError ErrorType = iota
	TransformationError
	DatabaseError
	NetworkError
	UnknownError
)

// MigrationError represents a detailed error with context
type MigrationError struct {
	Type        ErrorType
	DocumentID  string
	Message     string
	OriginalErr error
	Timestamp   time.Time
	Retryable   bool
}

func (e MigrationError) Error() string {
	return fmt.Sprintf("[%s] Document %s: %s (retryable: %t)",
		e.Type.String(), e.DocumentID, e.Message, e.Retryable)
}

func (et ErrorType) String() string {
	switch et {
	case ValidationError:
		return "VALIDATION"
	case TransformationError:
		return "TRANSFORMATION"
	case DatabaseError:
		return "DATABASE"
	case NetworkError:
		return "NETWORK"
	case UnknownError:
		return "UNKNOWN"
	default:
		return "UNKNOWN"
	}
}

// RetryableError represents an error that can be retried
type RetryableError struct {
	Err        error
	RetryCount int
	MaxRetries int
}

func (r RetryableError) Error() string {
	return fmt.Sprintf("retryable error (attempt %d/%d): %v", r.RetryCount, r.MaxRetries, r.Err)
}

// BatchProcessor handles batch processing with retry logic
type BatchProcessor struct {
	maxRetries   int
	retryDelay   time.Duration
	batchTimeout time.Duration
	errorLogger  *ErrorLogger
}

// ErrorLogger handles comprehensive error logging
type ErrorLogger struct {
	errorCounts map[ErrorType]int64
	mu          sync.RWMutex
}

func NewErrorLogger() *ErrorLogger {
	return &ErrorLogger{
		errorCounts: make(map[ErrorType]int64),
	}
}

func (el *ErrorLogger) LogError(err MigrationError) {
	el.mu.Lock()
	defer el.mu.Unlock()

	el.errorCounts[err.Type]++

	// Log to standard logger with structured format
	log.Printf("[ERROR] %s | Doc: %s | %s | Retryable: %t | Time: %s",
		err.Type.String(),
		err.DocumentID,
		err.Message,
		err.Retryable,
		err.Timestamp.Format(time.RFC3339))
}

func (el *ErrorLogger) GetErrorSummary() map[ErrorType]int64 {
	el.mu.RLock()
	defer el.mu.RUnlock()

	summary := make(map[ErrorType]int64)
	for k, v := range el.errorCounts {
		summary[k] = v
	}
	return summary
}

// ProgressTracker tracks migration progress
type ProgressTracker struct {
	totalDocuments     int64
	processedDocuments int64
	errorCount         int64
	startTime          time.Time
	mu                 sync.RWMutex
}

// NewProgressTracker creates a new progress tracker
func NewProgressTracker(totalDocuments int64) *ProgressTracker {
	return &ProgressTracker{
		totalDocuments: totalDocuments,
		startTime:      time.Now(),
	}
}

// SetTotal sets the total document count
func (p *ProgressTracker) SetTotal(total int64) {
	p.mu.Lock()
	defer p.mu.Unlock()
	p.totalDocuments = total
}

// IncrementProcessed increments the processed document count
func (p *ProgressTracker) IncrementProcessed(count int) {
	p.mu.Lock()
	defer p.mu.Unlock()
	p.processedDocuments += int64(count)
}

// IncrementErrors increments the error count
func (p *ProgressTracker) IncrementErrors(count int) {
	p.mu.Lock()
	defer p.mu.Unlock()
	p.errorCount += int64(count)
}

// GetProgress returns current progress statistics
func (p *ProgressTracker) GetProgress() (processed, total, errors int64, elapsed time.Duration) {
	p.mu.RLock()
	defer p.mu.RUnlock()
	return p.processedDocuments, p.totalDocuments, p.errorCount, time.Since(p.startTime)
}

// NewMigrationTool creates a new migration tool instance
func NewMigrationTool(config *Config) (*MigrationTool, error) {
	// Initialize MongoDB client
	mongoClient, err := NewMongoDBClient(config.MongoDB)
	if err != nil {
		return nil, fmt.Errorf("failed to create MongoDB client: %w", err)
	}

	// Initialize PostgreSQL client
	postgresClient, err := NewPostgreSQLClient(config.PostgreSQL, config.Migration)
	if err != nil {
		mongoClient.Close(context.Background())
		return nil, fmt.Errorf("failed to create PostgreSQL client: %w", err)
	}

	// Initialize error logger
	errorLogger := NewErrorLogger()

	// Initialize batch processor with configuration
	batchProcessor := &BatchProcessor{
		maxRetries:   config.Migration.MaxRetries,
		retryDelay:   time.Duration(config.Migration.RetryDelaySeconds) * time.Second,
		batchTimeout: time.Duration(config.Migration.BatchTimeoutSeconds) * time.Second,
		errorLogger:  errorLogger,
	}

	return &MigrationTool{
		config:         config,
		mongoClient:    mongoClient,
		postgresClient: postgresClient,
		batchProcessor: batchProcessor,
		errorLogger:    errorLogger,
		startTime:      time.Now(),
	}, nil
}

// Close closes all connections
func (m *MigrationTool) Close() error {
	var errors []error

	if err := m.mongoClient.Close(context.Background()); err != nil {
		errors = append(errors, fmt.Errorf("failed to close MongoDB client: %w", err))
	}

	if err := m.postgresClient.Close(); err != nil {
		errors = append(errors, fmt.Errorf("failed to close PostgreSQL client: %w", err))
	}

	if len(errors) > 0 {
		return fmt.Errorf("errors closing connections: %v", errors)
	}

	return nil
}

// Migrate performs the complete migration process
func (m *MigrationTool) Migrate(ctx context.Context) error {
	log.Println("Starting migration process...")

	// Total count will be determined by the resume function
	m.totalDocuments = 0                     // Will be set by resume function
	progressTracker := NewProgressTracker(0) // Will be updated

	// Create channels for communication between goroutines
	documentChan := make(chan []ThreatDocument, m.config.Migration.BufferSize)
	resultChan := make(chan MigrationResult, m.config.Migration.BufferSize)

	// Start worker pool
	var wg sync.WaitGroup
	for i := 0; i < m.config.Migration.WorkerCount; i++ {
		wg.Add(1)
		go m.worker(ctx, documentChan, resultChan, &wg)
	}

	// Start progress reporter
	progressCtx, progressCancel := context.WithCancel(ctx)
	go m.reportProgress(progressCtx, progressTracker)

	// Start result processor
	go m.processResults(ctx, resultChan, progressTracker)

	// Start reading documents from MongoDB with resume capability
	go func() {
		defer func() {
			// Safely close the channel
			select {
			case <-documentChan:
				// Channel already closed
			default:
				close(documentChan)
			}
		}()
		migrationName := "threat_intelligence_migration"
		if err := m.mongoClient.ReadAllDocumentsWithResume(ctx, m.config.Migration.BatchSize, documentChan, migrationName, m.postgresClient, progressTracker); err != nil {
			log.Printf("Error reading documents: %v", err)
		}
	}()

	// Wait for all workers to complete
	wg.Wait()
	close(resultChan)

	// Stop progress reporting
	progressCancel()

	// Generate comprehensive migration summary
	m.generateMigrationSummary(progressTracker)

	return nil
}

// worker processes batches of documents
func (m *MigrationTool) worker(ctx context.Context, documentChan <-chan []ThreatDocument, resultChan chan<- MigrationResult, wg *sync.WaitGroup) {
	defer wg.Done()

	for {
		select {
		case batch, ok := <-documentChan:
			if !ok {
				return // Channel closed, worker done
			}

			result := m.processBatch(ctx, batch)

			select {
			case resultChan <- result:
			case <-ctx.Done():
				return
			}

		case <-ctx.Done():
			return
		}
	}
}

// processBatch processes a batch of MongoDB documents with enhanced error handling and retry logic
func (m *MigrationTool) processBatch(ctx context.Context, documents []ThreatDocument) MigrationResult {
	startTime := time.Now()

	result := MigrationResult{
		ProcessedCount: 0,
		ErrorCount:     0,
		Errors:         make([]error, 0),
		ProcessingTime: 0,
		RetryCount:     0,
	}

	// Process batch with timeout
	batchCtx, cancel := context.WithTimeout(ctx, m.batchProcessor.batchTimeout)
	defer cancel()

	result = m.processBatchWithRetry(batchCtx, documents, 0)
	result.ProcessingTime = time.Since(startTime)

	return result
}

// processBatchWithRetry processes a batch with retry logic
func (m *MigrationTool) processBatchWithRetry(ctx context.Context, documents []ThreatDocument, retryCount int) MigrationResult {
	result := MigrationResult{
		ProcessedCount: 0,
		ErrorCount:     0,
		Errors:         make([]error, 0),
		RetryCount:     retryCount,
	}

	var threats []ThreatRecord
	var validationErrors []MigrationError
	var transformationErrors []MigrationError

	// Phase 1: Validate and transform documents
	for _, doc := range documents {
		select {
		case <-ctx.Done():
			result.Errors = append(result.Errors, fmt.Errorf("batch processing cancelled: %w", ctx.Err()))
			return result
		default:
		}

		// Validate document
		if err := doc.ValidateDocument(); err != nil {
			migErr := MigrationError{
				Type:        ValidationError,
				DocumentID:  doc.ID.Hex(),
				Message:     err.Error(),
				OriginalErr: err,
				Timestamp:   time.Now(),
				Retryable:   false, // Validation errors are not retryable
			}
			validationErrors = append(validationErrors, migErr)
			m.errorLogger.LogError(migErr)
			result.ErrorCount++
			continue
		}

		// Transform document
		threat, err := m.postgresClient.TransformDocument(doc)
		if err != nil {
			// Determine if transformation error is retryable
			retryable := m.isRetryableError(err)

			migErr := MigrationError{
				Type:        TransformationError,
				DocumentID:  doc.ID.Hex(),
				Message:     err.Error(),
				OriginalErr: err,
				Timestamp:   time.Now(),
				Retryable:   retryable,
			}
			transformationErrors = append(transformationErrors, migErr)
			m.errorLogger.LogError(migErr)
			result.ErrorCount++
			continue
		}

		threats = append(threats, *threat)
	}

	// Phase 2: Insert batch into PostgreSQL with retry logic
	if len(threats) > 0 {
		if m.config.Migration.DryRun {
			// Simulate success without DB writes
			result.ProcessedCount = len(threats)
		} else {
			insertErr := m.insertBatchWithRetry(ctx, threats, retryCount)
			if insertErr != nil {
				// Check if we should retry the entire batch
				if m.shouldRetryBatch(insertErr, retryCount) {
					log.Printf("Retrying batch (attempt %d/%d) after error: %v",
						retryCount+1, m.batchProcessor.maxRetries, insertErr)

					// Wait before retry
					select {
					case <-time.After(m.batchProcessor.retryDelay):
					case <-ctx.Done():
						result.Errors = append(result.Errors, fmt.Errorf("retry cancelled: %w", ctx.Err()))
						return result
					}

					// Retry the entire batch
					return m.processBatchWithRetry(ctx, documents, retryCount+1)
				}

				// Log database error for all documents in batch
				for _, threat := range threats {
					migErr := MigrationError{Type: DatabaseError, DocumentID: threat.ID.String(), Message: insertErr.Error(), OriginalErr: insertErr, Timestamp: time.Now(), Retryable: m.isRetryableError(insertErr)}
					m.errorLogger.LogError(migErr)
				}

				result.ErrorCount += len(threats)
				result.Errors = append(result.Errors, fmt.Errorf("batch insert failed after %d retries: %w", retryCount, insertErr))
			} else {
				result.ProcessedCount = len(threats)
			}
		}
	}

	// Add all collected errors to result
	for _, err := range validationErrors {
		result.Errors = append(result.Errors, err)
	}
	for _, err := range transformationErrors {
		result.Errors = append(result.Errors, err)
	}

	return result
}

// insertBatchWithRetry attempts to insert a batch with exponential backoff retry
func (m *MigrationTool) insertBatchWithRetry(ctx context.Context, threats []ThreatRecord, retryCount int) error {
	var lastErr error

	for attempt := 0; attempt <= m.batchProcessor.maxRetries; attempt++ {
		select {
		case <-ctx.Done():
			return fmt.Errorf("insert cancelled: %w", ctx.Err())
		default:
		}

		err := m.postgresClient.InsertThreatBatch(threats)
		if err == nil {
			return nil // Success
		}

		lastErr = err

		// Check if error is retryable
		if !m.isRetryableError(err) {
			return fmt.Errorf("non-retryable error: %w", err)
		}

		if attempt < m.batchProcessor.maxRetries {
			// Calculate exponential backoff delay
			delay := time.Duration(attempt+1) * m.batchProcessor.retryDelay
			log.Printf("Insert attempt %d failed, retrying in %v: %v", attempt+1, delay, err)

			select {
			case <-time.After(delay):
			case <-ctx.Done():
				return fmt.Errorf("retry cancelled: %w", ctx.Err())
			}
		}
	}

	return fmt.Errorf("insert failed after %d attempts: %w", m.batchProcessor.maxRetries+1, lastErr)
}

// isRetryableError determines if an error is retryable
func (m *MigrationTool) isRetryableError(err error) bool {
	if err == nil {
		return false
	}

	errStr := err.Error()

	// Network-related errors are typically retryable
	retryablePatterns := []string{
		"connection refused",
		"connection reset",
		"timeout",
		"temporary failure",
		"network is unreachable",
		"no such host",
		"connection timed out",
		"i/o timeout",
		"broken pipe",
		"connection lost",
		"server closed the connection",
		"deadlock detected",
		"lock wait timeout",
		"too many connections",
	}

	for _, pattern := range retryablePatterns {
		if contains(errStr, pattern) {
			return true
		}
	}

	return false
}

// shouldRetryBatch determines if an entire batch should be retried
func (m *MigrationTool) shouldRetryBatch(err error, currentRetryCount int) bool {
	if currentRetryCount >= m.batchProcessor.maxRetries {
		return false
	}

	return m.isRetryableError(err)
}

// contains checks if a string contains a substring (case-insensitive)
func contains(s, substr string) bool {
	return len(s) >= len(substr) &&
		(s == substr ||
			len(s) > len(substr) &&
				(s[:len(substr)] == substr ||
					s[len(s)-len(substr):] == substr ||
					containsSubstring(s, substr)))
}

func containsSubstring(s, substr string) bool {
	for i := 0; i <= len(s)-len(substr); i++ {
		if s[i:i+len(substr)] == substr {
			return true
		}
	}
	return false
}

// processResults processes migration results
func (m *MigrationTool) processResults(ctx context.Context, resultChan <-chan MigrationResult, progressTracker *ProgressTracker) {
	for {
		select {
		case result, ok := <-resultChan:
			if !ok {
				return // Channel closed
			}

			progressTracker.IncrementProcessed(result.ProcessedCount)
			progressTracker.IncrementErrors(result.ErrorCount)

			// Log errors
			for _, err := range result.Errors {
				log.Printf("Migration error: %v", err)
			}

		case <-ctx.Done():
			return
		}
	}
}

// reportProgress periodically reports migration progress with enhanced metrics
func (m *MigrationTool) reportProgress(ctx context.Context, progressTracker *ProgressTracker) {
	interval := time.Duration(m.config.Migration.ProgressReportInterval) * time.Second
	ticker := time.NewTicker(interval)
	defer ticker.Stop()

	for {
		select {
		case <-ticker.C:
			processed, total, errors, elapsed := progressTracker.GetProgress()
			if total > 0 {
				percentage := float64(processed) / float64(total) * 100
				rate := float64(processed) / elapsed.Seconds()

				var eta time.Duration
				if rate > 0 {
					remaining := total - processed
					eta = time.Duration(float64(remaining)/rate) * time.Second
				}

				// Get error breakdown
				errorSummary := m.errorLogger.GetErrorSummary()

				log.Printf("Progress: %.2f%% (%d/%d), Rate: %.1f docs/sec, Total Errors: %d, ETA: %v",
					percentage, processed, total, rate, errors, eta.Round(time.Second))

				// Log error breakdown if there are errors
				if errors > 0 {
					log.Printf("Error Breakdown - Validation: %d, Transformation: %d, Database: %d, Network: %d, Unknown: %d",
						errorSummary[ValidationError],
						errorSummary[TransformationError],
						errorSummary[DatabaseError],
						errorSummary[NetworkError],
						errorSummary[UnknownError])
				}
			}

		case <-ctx.Done():
			return
		}
	}
}

// generateMigrationSummary generates a comprehensive migration summary
func (m *MigrationTool) generateMigrationSummary(progressTracker *ProgressTracker) {
	processed, total, errors, elapsed := progressTracker.GetProgress()
	errorSummary := m.errorLogger.GetErrorSummary()

	log.Println(strings.Repeat("=", 80))
	log.Println("MIGRATION SUMMARY")
	log.Println(strings.Repeat("=", 80))

	// Overall statistics
	successRate := float64(processed) / float64(total) * 100
	errorRate := float64(errors) / float64(total) * 100
	avgRate := float64(processed) / elapsed.Seconds()

	log.Printf("Total Documents: %d", total)
	log.Printf("Successfully Processed: %d (%.2f%%)", processed, successRate)
	log.Printf("Failed Documents: %d (%.2f%%)", errors, errorRate)
	log.Printf("Total Elapsed Time: %v", elapsed.Round(time.Second))
	log.Printf("Average Processing Rate: %.1f docs/sec", avgRate)

	// Error breakdown
	if errors > 0 {
		log.Println("\nERROR BREAKDOWN:")
		log.Printf("  Validation Errors: %d", errorSummary[ValidationError])
		log.Printf("  Transformation Errors: %d", errorSummary[TransformationError])
		log.Printf("  Database Errors: %d", errorSummary[DatabaseError])
		log.Printf("  Network Errors: %d", errorSummary[NetworkError])
		log.Printf("  Unknown Errors: %d", errorSummary[UnknownError])
	}

	// Performance metrics
	log.Println("\nPERFORMANCE METRICS:")
	log.Printf("  Worker Count: %d", m.config.Migration.WorkerCount)
	log.Printf("  Batch Size: %d", m.config.Migration.BatchSize)
	log.Printf("  Buffer Size: %d", m.config.Migration.BufferSize)
	log.Printf("  Max Retries: %d", m.config.Migration.MaxRetries)

	// Migration status
	log.Println("\nMIGRATION STATUS:")
	if errors == 0 {
		log.Println("  ✅ COMPLETED SUCCESSFULLY - All documents migrated without errors")
	} else if processed > 0 {
		log.Printf("  ⚠️  COMPLETED WITH ERRORS - %d documents failed to migrate", errors)
		log.Println("     Check error logs above for detailed error information")
	} else {
		log.Println("  ❌ FAILED - No documents were successfully migrated")
	}

	log.Println(strings.Repeat("=", 80))
}

// HealthCheck performs a basic health check of the migration tool
func (m *MigrationTool) HealthCheck(ctx context.Context) error {
	log.Println("Performing health check...")

	// Check MongoDB connection
	if err := m.mongoClient.client.Ping(ctx, nil); err != nil {
		return fmt.Errorf("MongoDB health check failed: %w", err)
	}
	log.Println("✅ MongoDB connection healthy")

	// Check PostgreSQL connection
	if err := m.postgresClient.db.PingContext(ctx); err != nil {
		return fmt.Errorf("PostgreSQL health check failed: %w", err)
	}
	log.Println("✅ PostgreSQL connection healthy")

	// Check document count
	count, err := m.mongoClient.GetTotalDocumentCount(ctx)
	if err != nil {
		return fmt.Errorf("failed to get document count: %w", err)
	}
	log.Printf("✅ MongoDB document count: %d", count)

	log.Println("Health check completed successfully")
	return nil
}

func main() {
	// Check for test flag
	if len(os.Args) > 1 && os.Args[1] == "test" {
		TestTransformation()
		return
	}

	// Check for batch processing test flag
	if len(os.Args) > 1 && os.Args[1] == "test-batch" {
		TestBatchProcessing()
		TestBatchProcessingWithSampleData()
		return
	}

	// Check for UTF-8 sanitization test flag
	if len(os.Args) > 1 && os.Args[1] == "test-utf8" {
		TestUTF8Sanitization()
		TestNormalizationWithSanitization()
		return
	}

	// Check for aggressive sanitization test flag
	if len(os.Args) > 1 && os.Args[1] == "test-aggressive" {
		TestAggressiveSanitization()
		return
	}

	// Check for health check flag
	if len(os.Args) > 1 && os.Args[1] == "health" {
		config, err := LoadConfig()
		if err != nil {
			log.Fatalf("Failed to load configuration: %v", err)
		}

		migrationTool, err := NewMigrationTool(config)
		if err != nil {
			log.Fatalf("Failed to create migration tool: %v", err)
		}
		defer migrationTool.Close()

		ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
		defer cancel()

		if err := migrationTool.HealthCheck(ctx); err != nil {
			log.Fatalf("Health check failed: %v", err)
		}
		return
	}

	// Load configuration
	config, err := LoadConfig()
	if err != nil {
		log.Fatalf("Failed to load configuration: %v", err)
	}

	// Create migration tool
	migrationTool, err := NewMigrationTool(config)
	if err != nil {
		log.Fatalf("Failed to create migration tool: %v", err)
	}
	defer migrationTool.Close()

	// Set up context with cancellation for graceful shutdown
	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()

	// Handle interrupt signals for graceful shutdown
	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)

	go func() {
		<-sigChan
		log.Println("Received interrupt signal, shutting down gracefully...")
		cancel()
	}()

	// Start migration
	if err := migrationTool.Migrate(ctx); err != nil {
		log.Fatalf("Migration failed: %v", err)
	}

	log.Println("Migration completed successfully!")
}
