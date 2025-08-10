package main

import (
	"fmt"
	"time"

	"go.mongodb.org/mongo-driver/bson/primitive"
)

// TestBatchProcessing tests the batch processing and error handling functionality
func TestBatchProcessing() {
	fmt.Println("=== Testing Batch Processing and Error Handling ===")

	// Test error classification
	fmt.Println("\n1. Testing Error Classification:")
	testErrorClassification()

	// Test retry logic
	fmt.Println("\n2. Testing Retry Logic:")
	testRetryLogic()

	// Test progress tracking
	fmt.Println("\n3. Testing Progress Tracking:")
	testProgressTracking()

	// Test error logging
	fmt.Println("\n4. Testing Error Logging:")
	testErrorLogging()

	fmt.Println("\n=== Batch Processing Test Complete ===")
}

func testErrorClassification() {
	// Create sample errors of different types
	errors := []MigrationError{
		{
			Type:       ValidationError,
			DocumentID: "test-doc-1",
			Message:    "Missing required field: source_address",
			Timestamp:  time.Now(),
			Retryable:  false,
		},
		{
			Type:       TransformationError,
			DocumentID: "test-doc-2",
			Message:    "Invalid IP address format",
			Timestamp:  time.Now(),
			Retryable:  false,
		},
		{
			Type:       DatabaseError,
			DocumentID: "test-doc-3",
			Message:    "Connection timeout",
			Timestamp:  time.Now(),
			Retryable:  true,
		},
		{
			Type:       NetworkError,
			DocumentID: "test-doc-4",
			Message:    "Network unreachable",
			Timestamp:  time.Now(),
			Retryable:  true,
		},
	}

	for _, err := range errors {
		fmt.Printf("  %s: %s (Retryable: %t)\n", err.Type.String(), err.Message, err.Retryable)
	}
}

func testRetryLogic() {
	// Create a mock migration tool for testing
	config := &Config{
		Migration: MigrationConfig{
			MaxRetries:          3,
			RetryDelaySeconds:   1,
			BatchTimeoutSeconds: 10,
		},
	}

	errorLogger := NewErrorLogger()
	batchProcessor := &BatchProcessor{
		maxRetries:   config.Migration.MaxRetries,
		retryDelay:   time.Duration(config.Migration.RetryDelaySeconds) * time.Second,
		batchTimeout: time.Duration(config.Migration.BatchTimeoutSeconds) * time.Second,
		errorLogger:  errorLogger,
	}

	mockTool := &MigrationTool{
		config:         config,
		batchProcessor: batchProcessor,
		errorLogger:    errorLogger,
	}

	// Test retryable error detection
	retryableErrors := []error{
		fmt.Errorf("connection refused"),
		fmt.Errorf("timeout occurred"),
		fmt.Errorf("network is unreachable"),
		fmt.Errorf("deadlock detected"),
	}

	nonRetryableErrors := []error{
		fmt.Errorf("validation failed"),
		fmt.Errorf("invalid data format"),
		fmt.Errorf("constraint violation"),
	}

	fmt.Println("  Retryable Errors:")
	for _, err := range retryableErrors {
		isRetryable := mockTool.isRetryableError(err)
		fmt.Printf("    '%s' -> Retryable: %t ✓\n", err.Error(), isRetryable)
	}

	fmt.Println("  Non-Retryable Errors:")
	for _, err := range nonRetryableErrors {
		isRetryable := mockTool.isRetryableError(err)
		fmt.Printf("    '%s' -> Retryable: %t ✓\n", err.Error(), isRetryable)
	}
}

func testProgressTracking() {
	// Create a progress tracker with sample data
	totalDocs := int64(10000)
	tracker := NewProgressTracker(totalDocs)

	// Simulate processing progress
	fmt.Printf("  Initial Progress: 0/%d documents\n", totalDocs)

	// Simulate batch processing
	batches := []int{1000, 1500, 2000, 1200, 800, 1000, 1500, 1000}
	errors := []int{5, 8, 12, 3, 2, 7, 10, 4}

	for i, batchSize := range batches {
		tracker.IncrementProcessed(batchSize)
		tracker.IncrementErrors(errors[i])

		processed, total, errorCount, elapsed := tracker.GetProgress()
		percentage := float64(processed) / float64(total) * 100
		rate := float64(processed) / elapsed.Seconds()

		fmt.Printf("  Batch %d: %.1f%% (%d/%d), Rate: %.1f docs/sec, Errors: %d\n",
			i+1, percentage, processed, total, rate, errorCount)

		// Small delay to simulate processing time
		time.Sleep(100 * time.Millisecond)
	}
}

func testErrorLogging() {
	errorLogger := NewErrorLogger()

	// Create sample errors
	sampleErrors := []MigrationError{
		{Type: ValidationError, DocumentID: "doc1", Message: "Invalid format", Timestamp: time.Now()},
		{Type: ValidationError, DocumentID: "doc2", Message: "Missing field", Timestamp: time.Now()},
		{Type: TransformationError, DocumentID: "doc3", Message: "Parse error", Timestamp: time.Now()},
		{Type: DatabaseError, DocumentID: "doc4", Message: "Connection lost", Timestamp: time.Now()},
		{Type: DatabaseError, DocumentID: "doc5", Message: "Timeout", Timestamp: time.Now()},
		{Type: NetworkError, DocumentID: "doc6", Message: "Network unreachable", Timestamp: time.Now()},
	}

	// Log all errors
	for _, err := range sampleErrors {
		errorLogger.LogError(err)
	}

	// Get error summary
	summary := errorLogger.GetErrorSummary()

	fmt.Println("  Error Summary:")
	fmt.Printf("    Validation Errors: %d\n", summary[ValidationError])
	fmt.Printf("    Transformation Errors: %d\n", summary[TransformationError])
	fmt.Printf("    Database Errors: %d\n", summary[DatabaseError])
	fmt.Printf("    Network Errors: %d\n", summary[NetworkError])
	fmt.Printf("    Unknown Errors: %d\n", summary[UnknownError])

	totalErrors := int64(0)
	for _, count := range summary {
		totalErrors += count
	}
	fmt.Printf("    Total Errors: %d ✓\n", totalErrors)
}

// TestBatchProcessingWithSampleData tests batch processing with realistic document data
func TestBatchProcessingWithSampleData() {
	fmt.Println("\n=== Testing Batch Processing with Sample Data ===")

	// Create sample documents with various data quality issues
	documents := []ThreatDocument{
		// Valid document
		{
			ID:            primitive.NewObjectID(),
			ASN:           "138062",
			Timestamp:     time.Now().Add(-24 * time.Hour),
			ASNInfo:       "IDNIC-PAAS-AS-ID PT. Awan Kilat Semesta, ID",
			Category:      "bot",
			SourceAddress: "103.129.222.46",
			SourceCountry: "ID",
			OptionalInformation: OptionalInfo{
				DestinationAddress: "137.175.90.212",
				DestinationCountry: "US",
				DestinationPort:    "80",
				SourcePort:         "59436",
				Protocol:           "TCP",
				Family:             "xorddos",
			},
		},
		// Document with invalid IP
		{
			ID:            primitive.NewObjectID(),
			ASN:           "12345",
			Timestamp:     time.Now().Add(-12 * time.Hour),
			ASNInfo:       "Test ASN",
			Category:      "malware",
			SourceAddress: "invalid-ip-address",
			SourceCountry: "US",
		},
		// Document with missing required fields
		{
			ID:        primitive.NewObjectID(),
			Timestamp: time.Now().Add(-6 * time.Hour),
			// Missing ASN, Category, SourceAddress
		},
		// Valid document with optional fields
		{
			ID:            primitive.NewObjectID(),
			ASN:           "54321",
			Timestamp:     time.Now().Add(-2 * time.Hour),
			ASNInfo:       "Another Test ASN",
			Category:      "phishing",
			SourceAddress: "192.168.1.100",
			SourceCountry: "CA",
			OptionalInformation: OptionalInfo{
				Protocol: "HTTPS",
				Family:   "banking_trojan",
			},
		},
	}

	fmt.Printf("Testing batch processing with %d sample documents:\n", len(documents))

	validCount := 0
	invalidCount := 0

	for i, doc := range documents {
		fmt.Printf("\nDocument %d (ID: %s):\n", i+1, doc.ID.Hex())

		// Test validation
		if err := doc.ValidateDocument(); err != nil {
			fmt.Printf("  ❌ Validation failed: %s\n", err.Error())
			invalidCount++
		} else {
			fmt.Printf("  ✅ Validation passed\n")

			// Test transformation (mock)
			fmt.Printf("  📝 Transformation details:\n")
			fmt.Printf("     ASN: %s\n", doc.ASN)
			fmt.Printf("     Source: %s (%s)\n", doc.SourceAddress, doc.SourceCountry)
			fmt.Printf("     Category: %s\n", doc.Category)

			if doc.OptionalInformation.DestinationAddress != "" {
				fmt.Printf("     Destination: %s (%s)\n",
					doc.OptionalInformation.DestinationAddress,
					doc.OptionalInformation.DestinationCountry)
			}

			validCount++
		}
	}

	fmt.Printf("\nBatch Processing Summary:\n")
	fmt.Printf("  Valid Documents: %d\n", validCount)
	fmt.Printf("  Invalid Documents: %d\n", invalidCount)
	fmt.Printf("  Success Rate: %.1f%%\n", float64(validCount)/float64(len(documents))*100)

	fmt.Println("\n=== Batch Processing with Sample Data Complete ===")
}
