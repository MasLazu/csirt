package main

import (
	"context"
	"fmt"
	"log"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"go.mongodb.org/mongo-driver/mongo/options"
)

// All migration progress tracking is now based on counting existing records in PostgreSQL

// GetLastProcessedObjectID converts the last processed ID string to ObjectID (kept for compatibility)
func GetLastProcessedObjectID(lastProcessedID string) (primitive.ObjectID, error) {
	if lastProcessedID == "" {
		return primitive.NilObjectID, nil
	}

	objectID, err := primitive.ObjectIDFromHex(lastProcessedID)
	if err != nil {
		return primitive.NilObjectID, fmt.Errorf("invalid ObjectID format: %w", err)
	}

	return objectID, nil
}

// ReadDocumentsBatchWithSkip reads documents starting from a specific skip count
func (m *MongoDBClient) ReadDocumentsBatchWithSkip(ctx context.Context, batchSize int, skip int64) ([]ThreatDocument, error) {
	filter := bson.M{}

	options := options.Find().
		SetLimit(int64(batchSize)).
		SetSkip(skip).
		SetSort(bson.D{{Key: "_id", Value: 1}}) // Sort by _id for consistent ordering

	cursor, err := m.collection.Find(ctx, filter, options)
	if err != nil {
		return nil, fmt.Errorf("failed to find documents: %w", err)
	}
	defer cursor.Close(ctx)

	var documents []ThreatDocument
	for cursor.Next(ctx) {
		var doc ThreatDocument
		if err := cursor.Decode(&doc); err != nil {
			log.Printf("Warning: failed to decode document: %v", err)
			continue // Skip malformed documents
		}
		documents = append(documents, doc)
	}

	if err := cursor.Err(); err != nil {
		return nil, fmt.Errorf("cursor error: %w", err)
	}

	return documents, nil
}

// GetExistingRecordCount returns the count of existing records in PostgreSQL
func (p *PostgreSQLClient) GetExistingRecordCount() (int64, error) {
	var count int64
	err := p.db.QueryRow("SELECT COUNT(*) FROM threat_intelligence").Scan(&count)
	if err != nil {
		return 0, fmt.Errorf("failed to count existing records: %w", err)
	}
	return count, nil
}

// ReadAllDocumentsWithResume reads all documents with simple count-based resume
func (m *MongoDBClient) ReadAllDocumentsWithResume(ctx context.Context, batchSize int, documentChan chan<- []ThreatDocument, migrationName string, postgresClient *PostgreSQLClient) error {
	defer close(documentChan)

	log.Println("Checking existing records in PostgreSQL...")
	// Get existing record count from PostgreSQL first (this is fast)
	existingCount, err := postgresClient.GetExistingRecordCount()
	if err != nil {
		return fmt.Errorf("failed to get existing record count: %w", err)
	}
	log.Printf("Found %d existing records in PostgreSQL", existingCount)

	// Use estimated count for total (much faster than exact count)
	log.Println("Getting estimated total document count from MongoDB...")
	totalCount, err := m.collection.EstimatedDocumentCount(ctx)
	if err != nil {
		log.Printf("Estimated count failed, using fallback: %v", err)
		totalCount = 25121993 // Use known total as fallback
	}
	log.Printf("Total documents to process: %d", totalCount)

	if existingCount > 0 {
		log.Printf("🔄 RESUMING migration from count: %d/%d (%.2f%%) already processed",
			existingCount, totalCount, float64(existingCount)/float64(totalCount)*100)
	} else {
		log.Printf("🚀 STARTING new migration of %d documents", totalCount)
	}

	currentSkip := existingCount // Start from where we left off

	for currentSkip < totalCount {
		select {
		case <-ctx.Done():
			return ctx.Err()
		default:
		}

		batch, err := m.ReadDocumentsBatchWithSkip(ctx, batchSize, currentSkip)
		if err != nil {
			return fmt.Errorf("failed to read batch at skip %d: %w", currentSkip, err)
		}

		if len(batch) == 0 {
			log.Printf("No more documents found at skip %d", currentSkip)
			break
		}

		select {
		case documentChan <- batch:
		case <-ctx.Done():
			return ctx.Err()
		}

		// Update progress tracking
		currentSkip += int64(len(batch))

		// Log progress every batch
		progress := float64(currentSkip) / float64(totalCount) * 100
		log.Printf("📖 Read progress: %.2f%% (%d/%d documents), Skip: %d",
			progress, currentSkip, totalCount, currentSkip)
	}

	log.Printf("✅ Migration reading completed: %d documents processed", currentSkip)
	return nil
}
