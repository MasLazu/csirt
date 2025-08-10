package main

import (
	"context"
	"fmt"
	"log"
	"time"

	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

// ThreatDocument represents the MongoDB document structure
type ThreatDocument struct {
	ID                  primitive.ObjectID `bson:"_id"`
	ASN                 string             `bson:"asn"`
	Timestamp           time.Time          `bson:"timestamp"`
	ASNInfo             string             `bson:"asninfo"`
	OptionalInformation OptionalInfo       `bson:"optional_information"`
	Category            string             `bson:"category"`
	SourceAddress       string             `bson:"source_address"`
	SourceCountry       string             `bson:"source_country"`
	CreatedAt           time.Time          `bson:"created_at,omitempty"`
	UpdatedAt           time.Time          `bson:"updated_at,omitempty"`
}

// OptionalInfo represents the optional information embedded in MongoDB documents
type OptionalInfo struct {
	DestinationAddress string `bson:"destination_address,omitempty"`
	DestinationCountry string `bson:"destination_country,omitempty"`
	DestinationPort    string `bson:"destination_port,omitempty"`
	SourcePort         string `bson:"source_port,omitempty"`
	Protocol           string `bson:"protocol,omitempty"`
	Family             string `bson:"family,omitempty"`
}

// MongoDBClient wraps MongoDB operations
type MongoDBClient struct {
	client     *mongo.Client
	database   *mongo.Database
	collection *mongo.Collection
	config     MongoConfig
}

// NewMongoDBClient creates a new MongoDB client
func NewMongoDBClient(config MongoConfig) (*MongoDBClient, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	clientOptions := options.Client().ApplyURI(config.URI)
	client, err := mongo.Connect(ctx, clientOptions)
	if err != nil {
		return nil, fmt.Errorf("failed to connect to MongoDB: %w", err)
	}

	// Test the connection
	if err := client.Ping(ctx, nil); err != nil {
		return nil, fmt.Errorf("failed to ping MongoDB: %w", err)
	}

	database := client.Database(config.Database)
	collection := database.Collection(config.Collection)

	log.Printf("Connected to MongoDB: %s/%s", config.Database, config.Collection)

	return &MongoDBClient{
		client:     client,
		database:   database,
		collection: collection,
		config:     config,
	}, nil
}

// Close closes the MongoDB connection
func (m *MongoDBClient) Close(ctx context.Context) error {
	return m.client.Disconnect(ctx)
}

// GetTotalDocumentCount returns the total number of documents in the collection
func (m *MongoDBClient) GetTotalDocumentCount(ctx context.Context) (int64, error) {
	// Use exact count with the same filter as ReadDocumentsBatch to ensure consistency
	log.Println("Counting non-deleted documents (this may take a moment)...")
	return m.getExactDocumentCount(ctx)
}

// getExactDocumentCount returns exact count with timeout
func (m *MongoDBClient) getExactDocumentCount(ctx context.Context) (int64, error) {
	// Create a context with timeout for the count operation
	countCtx, cancel := context.WithTimeout(ctx, 30*time.Second)
	defer cancel()

	// Count all documents (no filter needed since there's no deleted_at field)
	count, err := m.collection.CountDocuments(countCtx, bson.M{})
	if err != nil {
		return 0, fmt.Errorf("failed to count documents: %w", err)
	}

	return count, nil
}

// ReadDocumentsBatch reads documents in batches from MongoDB
func (m *MongoDBClient) ReadDocumentsBatch(ctx context.Context, batchSize int, skip int64) ([]ThreatDocument, error) {
	// No filter needed since there's no deleted_at field in the collection
	filter := bson.M{}

	options := options.Find().
		SetLimit(int64(batchSize)).
		SetSkip(skip).
		SetSort(bson.D{{Key: "_id", Value: 1}}) // Sort by _id for consistent pagination

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

// ReadAllDocuments reads all documents from MongoDB using a channel for streaming
func (m *MongoDBClient) ReadAllDocuments(ctx context.Context, batchSize int, documentChan chan<- []ThreatDocument) error {
	defer close(documentChan)

	totalCount, err := m.GetTotalDocumentCount(ctx)
	if err != nil {
		return fmt.Errorf("failed to get total document count: %w", err)
	}

	log.Printf("Starting migration of %d documents from MongoDB", totalCount)

	var skip int64 = 0
	var consecutiveEmptyBatches = 0

	for skip < totalCount {
		select {
		case <-ctx.Done():
			return ctx.Err()
		default:
		}

		batch, err := m.ReadDocumentsBatch(ctx, batchSize, skip)
		if err != nil {
			return fmt.Errorf("failed to read batch at skip %d: %w", skip, err)
		}

		if len(batch) == 0 {
			consecutiveEmptyBatches++
			// Only break if we get multiple consecutive empty batches
			// This handles cases where there might be gaps in the data
			if consecutiveEmptyBatches >= 3 {
				log.Printf("Stopping after %d consecutive empty batches at skip %d", consecutiveEmptyBatches, skip)
				break
			}
			// Skip ahead to try to find more documents
			skip += int64(batchSize)
			continue
		}

		// Reset empty batch counter when we find documents
		consecutiveEmptyBatches = 0

		select {
		case documentChan <- batch:
		case <-ctx.Done():
			return ctx.Err()
		}

		skip += int64(len(batch))

		// Log progress
		progress := float64(skip) / float64(totalCount) * 100
		log.Printf("Read progress: %.2f%% (%d/%d documents)", progress, skip, totalCount)
	}

	log.Printf("Finished reading %d documents from MongoDB", skip)
	return nil
}

// ValidateDocument performs basic validation on a threat document
func (d *ThreatDocument) ValidateDocument() error {
	if d.ID.IsZero() {
		return fmt.Errorf("document ID is empty")
	}

	if d.Timestamp.IsZero() {
		return fmt.Errorf("timestamp is empty")
	}

	if d.SourceAddress == "" {
		return fmt.Errorf("source address is empty")
	}

	if d.Category == "" {
		return fmt.Errorf("category is empty")
	}

	return nil
}
