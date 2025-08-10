package main

import (
	"fmt"
	"log"
	"time"

	"go.mongodb.org/mongo-driver/bson/primitive"
)

// TestTransformation tests the data transformation logic with sample data
func TestTransformation() {
	// Create a sample MongoDB document similar to the real data
	sampleDoc := ThreatDocument{
		ID:        primitive.NewObjectID(),
		ASN:       "138062",
		Timestamp: time.Now().Add(-24 * time.Hour),
		ASNInfo:   "IDNIC-PAAS-AS-ID PT. Awan Kilat Semesta, ID",
		OptionalInformation: OptionalInfo{
			DestinationAddress: "137.175.90.212",
			DestinationCountry: "US",
			DestinationPort:    "80",
			SourcePort:         "59436",
			Protocol:           "TCP",
			Family:             "xorddos",
		},
		Category:      "bot",
		SourceAddress: "103.129.222.46",
		SourceCountry: "ID",
	}

	fmt.Println("=== Testing Data Transformation ===")
	fmt.Printf("Sample MongoDB Document:\n")
	fmt.Printf("  ID: %s\n", sampleDoc.ID.Hex())
	fmt.Printf("  ASN: %s\n", sampleDoc.ASN)
	fmt.Printf("  ASN Info: %s\n", sampleDoc.ASNInfo)
	fmt.Printf("  Timestamp: %s\n", sampleDoc.Timestamp.Format(time.RFC3339))
	fmt.Printf("  Source Address: %s\n", sampleDoc.SourceAddress)
	fmt.Printf("  Source Country: %s\n", sampleDoc.SourceCountry)
	fmt.Printf("  Category: %s\n", sampleDoc.Category)
	fmt.Printf("  Destination Address: %s\n", sampleDoc.OptionalInformation.DestinationAddress)
	fmt.Printf("  Destination Country: %s\n", sampleDoc.OptionalInformation.DestinationCountry)
	fmt.Printf("  Source Port: %s\n", sampleDoc.OptionalInformation.SourcePort)
	fmt.Printf("  Destination Port: %s\n", sampleDoc.OptionalInformation.DestinationPort)
	fmt.Printf("  Protocol: %s\n", sampleDoc.OptionalInformation.Protocol)
	fmt.Printf("  Malware Family: %s\n", sampleDoc.OptionalInformation.Family)

	// Test validation
	fmt.Printf("\n=== Testing Document Validation ===\n")
	if err := sampleDoc.ValidateDocument(); err != nil {
		log.Printf("Validation failed: %v", err)
	} else {
		fmt.Printf("✓ Document validation passed\n")
	}

	fmt.Printf("\n=== Testing Data Normalization ===\n")

	// Create a mock PostgreSQL client for testing normalization functions
	mockClient := &PostgreSQLClient{}

	// Test ASN normalization
	normalizedASN := mockClient.normalizeASN(sampleDoc.ASN)
	fmt.Printf("ASN: '%s' -> '%s'\n", sampleDoc.ASN, normalizedASN)

	// Test ASN info normalization
	normalizedASNInfo := mockClient.normalizeASNInfo(sampleDoc.ASNInfo)
	fmt.Printf("ASN Info: '%s' -> '%s'\n", sampleDoc.ASNInfo, normalizedASNInfo)

	// Test category normalization
	normalizedCategory := mockClient.normalizeCategory(sampleDoc.Category)
	fmt.Printf("Category: '%s' -> '%s'\n", sampleDoc.Category, normalizedCategory)

	// Test country code normalization
	normalizedSourceCountry := mockClient.normalizeCountryCode(sampleDoc.SourceCountry)
	fmt.Printf("Source Country: '%s' -> '%s'\n", sampleDoc.SourceCountry, normalizedSourceCountry)

	normalizedDestCountry := mockClient.normalizeCountryCode(sampleDoc.OptionalInformation.DestinationCountry)
	fmt.Printf("Destination Country: '%s' -> '%s'\n", sampleDoc.OptionalInformation.DestinationCountry, normalizedDestCountry)

	// Test protocol normalization
	normalizedProtocol := mockClient.normalizeProtocol(sampleDoc.OptionalInformation.Protocol)
	fmt.Printf("Protocol: '%s' -> '%s'\n", sampleDoc.OptionalInformation.Protocol, normalizedProtocol)

	// Test malware family normalization
	normalizedFamily := mockClient.normalizeMalwareFamily(sampleDoc.OptionalInformation.Family)
	fmt.Printf("Malware Family: '%s' -> '%s'\n", sampleDoc.OptionalInformation.Family, normalizedFamily)

	// Test IP address parsing
	fmt.Printf("\n=== Testing IP Address Parsing ===\n")
	sourceIP, err := mockClient.parseAndValidateIPAddress(sampleDoc.SourceAddress)
	if err != nil {
		log.Printf("Source IP parsing failed: %v", err)
	} else {
		fmt.Printf("✓ Source IP: '%s' -> %s\n", sampleDoc.SourceAddress, sourceIP.String())
	}

	destIP, err := mockClient.parseAndValidateIPAddress(sampleDoc.OptionalInformation.DestinationAddress)
	if err != nil {
		log.Printf("Destination IP parsing failed: %v", err)
	} else {
		fmt.Printf("✓ Destination IP: '%s' -> %s\n", sampleDoc.OptionalInformation.DestinationAddress, destIP.String())
	}

	// Test port parsing
	fmt.Printf("\n=== Testing Port Parsing ===\n")
	sourcePort, err := mockClient.parseAndValidatePort(sampleDoc.OptionalInformation.SourcePort)
	if err != nil {
		log.Printf("Source port parsing failed: %v", err)
	} else {
		fmt.Printf("✓ Source Port: '%s' -> %d\n", sampleDoc.OptionalInformation.SourcePort, sourcePort)
	}

	destPort, err := mockClient.parseAndValidatePort(sampleDoc.OptionalInformation.DestinationPort)
	if err != nil {
		log.Printf("Destination port parsing failed: %v", err)
	} else {
		fmt.Printf("✓ Destination Port: '%s' -> %d\n", sampleDoc.OptionalInformation.DestinationPort, destPort)
	}

	fmt.Printf("\n=== Transformation Test Complete ===\n")
}
