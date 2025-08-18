package main

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"net/url"
	"time"
)

// AbuseIPDBResponse represents the API response
type AbuseIPDBResponse struct {
	Data struct {
		IPAddress            string    `json:"ipAddress"`
		IsPublic             bool      `json:"isPublic"`
		IPVersion            int       `json:"ipVersion"`
		IsWhitelisted        bool      `json:"isWhitelisted"`
		AbuseConfidenceLevel int       `json:"abuseConfidencePercentage"`
		CountryCode          string    `json:"countryCode"`
		CountryName          string    `json:"countryName"`
		UsageType            string    `json:"usageType"`
		ISP                  string    `json:"isp"`
		Domain               string    `json:"domain"`
		TotalReports         int       `json:"totalReports"`
		NumDistinctUsers     int       `json:"numDistinctUsers"`
		LastReportedAt       time.Time `json:"lastReportedAt"`
	} `json:"data"`
}

// ThreatIntelligenceEnricher handles threat intelligence API calls
type ThreatIntelligenceEnricher struct {
	apiKey     string
	httpClient *http.Client
}

// NewThreatIntelligenceEnricher creates a new enricher
func NewThreatIntelligenceEnricher(apiKey string) *ThreatIntelligenceEnricher {
	return &ThreatIntelligenceEnricher{
		apiKey: apiKey,
		httpClient: &http.Client{
			Timeout: 10 * time.Second,
		},
	}
}

// CheckIPReputation checks IP reputation using AbuseIPDB
func (t *ThreatIntelligenceEnricher) CheckIPReputation(ipAddress string) (*AbuseIPDBResponse, error) {
	// AbuseIPDB API endpoint
	baseURL := "https://api.abuseipdb.com/api/v2/check"
	
	// Prepare request
	params := url.Values{}
	params.Add("ipAddress", ipAddress)
	params.Add("maxAgeInDays", "90")
	params.Add("verbose", "true")
	
	req, err := http.NewRequest("GET", baseURL+"?"+params.Encode(), nil)
	if err != nil {
		return nil, err
	}
	
	// Add headers
	req.Header.Add("Key", t.apiKey)
	req.Header.Add("Accept", "application/json")
	
	// Make request
	resp, err := t.httpClient.Do(req)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()
	
	// Read response
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}
	
	// Parse JSON
	var result AbuseIPDBResponse
	err = json.Unmarshal(body, &result)
	if err != nil {
		return nil, err
	}
	
	return &result, nil
}

// Example usage for enriching ThreatEvents
func EnrichThreatEventsWithIntelligence() {
	// Get your free API key from https://www.abuseipdb.com/api
	enricher := NewThreatIntelligenceEnricher("YOUR_API_KEY")
	
	// Example: Check a suspicious IP
	ipAddress := "192.168.1.1"
	intel, err := enricher.CheckIPReputation(ipAddress)
	if err != nil {
		fmt.Printf("Error checking IP %s: %v\n", ipAddress, err)
		return
	}
	
	fmt.Printf("IP: %s\n", intel.Data.IPAddress)
	fmt.Printf("Abuse Confidence: %d%%\n", intel.Data.AbuseConfidenceLevel)
	fmt.Printf("Country: %s (%s)\n", intel.Data.CountryName, intel.Data.CountryCode)
	fmt.Printf("ISP: %s\n", intel.Data.ISP)
	fmt.Printf("Total Reports: %d\n", intel.Data.TotalReports)
	
	// Update your ThreatEvents table with this intelligence
	updateSQL := `
		UPDATE "ThreatEvents" 
		SET "RiskScore" = $1, "SourceISP" = $2, "SourceOrganization" = $3,
			"ThreatTags" = $4
		WHERE "SourceAddress" = $5`
	
	// Calculate risk score based on abuse confidence
	riskScore := float64(intel.Data.AbuseConfidenceLevel)
	tags := []string{"abuseipdb"}
	if intel.Data.AbuseConfidenceLevel > 50 {
		tags = append(tags, "high-risk")
	}
	
	// Execute update with your database connection
	// db.Exec(updateSQL, riskScore, intel.Data.ISP, intel.Data.Domain, tags, ipAddress)
}

// Batch processing for large datasets
func BatchEnrichThreatEvents(ipAddresses []string) {
	enricher := NewThreatIntelligenceEnricher("YOUR_API_KEY")
	
	// Rate limiting: AbuseIPDB free tier allows 1000 requests/day
	rateLimiter := time.NewTicker(100 * time.Millisecond) // 10 requests/second max
	defer rateLimiter.Stop()
	
	for _, ip := range ipAddresses {
		<-rateLimiter.C // Wait for rate limiter
		
		intel, err := enricher.CheckIPReputation(ip)
		if err != nil {
			fmt.Printf("Error processing IP %s: %v\n", ip, err)
			continue
		}
		
		// Process and store intelligence data
		fmt.Printf("Processed IP %s with confidence %d%%\n", 
			ip, intel.Data.AbuseConfidenceLevel)
	}
}
