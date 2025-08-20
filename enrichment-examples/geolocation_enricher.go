package main

import (
	"encoding/csv"
	"fmt"
	"net"
	"os"
	"strconv"
)

// GeoLocation represents geographic data for an IP
type GeoLocation struct {
	CountryCode  string
	CountryName  string
	Region       string
	City         string
	Latitude     float64
	Longitude    float64
	ISP          string
	Organization string
}

// GeoLocationEnricher handles IP geolocation enrichment
type GeoLocationEnricher struct {
	cityDatabase map[string]GeoLocation
}

// NewGeoLocationEnricher creates a new enricher from MaxMind GeoLite2 CSV
func NewGeoLocationEnricher(csvPath string) (*GeoLocationEnricher, error) {
	enricher := &GeoLocationEnricher{
		cityDatabase: make(map[string]GeoLocation),
	}

	// Load MaxMind GeoLite2 CSV data
	file, err := os.Open(csvPath)
	if err != nil {
		return nil, err
	}
	defer file.Close()

	reader := csv.NewReader(file)
	records, err := reader.ReadAll()
	if err != nil {
		return nil, err
	}

	// Parse CSV records (skip header)
	for i, record := range records {
		if i == 0 {
			continue // Skip header
		}

		// MaxMind GeoLite2 City CSV format:
		// network,geoname_id,registered_country_geoname_id,represented_country_geoname_id,
		// is_anonymous_proxy,is_satellite_provider,postal_code,latitude,longitude,accuracy_radius
		if len(record) >= 8 {
			network := record[0]
			latitude, _ := strconv.ParseFloat(record[7], 64)
			longitude, _ := strconv.ParseFloat(record[8], 64)

			enricher.cityDatabase[network] = GeoLocation{
				Latitude:  latitude,
				Longitude: longitude,
				// You'll need to join with location CSV for city/country names
			}
		}
	}

	return enricher, nil
}

// EnrichIP adds geolocation data to an IP address
func (g *GeoLocationEnricher) EnrichIP(ipAddr string) *GeoLocation {
	ip := net.ParseIP(ipAddr)
	if ip == nil {
		return nil
	}

	// Find matching network range
	for network, location := range g.cityDatabase {
		_, cidr, err := net.ParseCIDR(network)
		if err != nil {
			continue
		}

		if cidr.Contains(ip) {
			return &location
		}
	}

	return nil
}

// Example usage for your ThreatEvents enrichment
func EnrichThreatEventsWithGeo() {
	enricher, err := NewGeoLocationEnricher("GeoLite2-City-Blocks-IPv4.csv")
	if err != nil {
		fmt.Printf("Error loading geo database: %v\n", err)
		return
	}

	// Example: Enrich a threat event IP
	sourceIP := "8.8.8.8"
	geoData := enricher.EnrichIP(sourceIP)
	if geoData != nil {
		fmt.Printf("IP %s located at: %f, %f\n", sourceIP, geoData.Latitude, geoData.Longitude)

		// Update your PostgreSQL with this data
		updateSQL := `
			UPDATE "ThreatEvents" 
			SET "SourceLatitude" = $1, "SourceLongitude" = $2, 
				"SourceCity" = $3, "SourceRegion" = $4 
			WHERE "SourceAddress" = $5`
		// Execute update...
	}
}
