package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"io"
	"net"
	"net/http"
	"os"
	"strconv"
	"strings"
	"time"
)

// ASN Intelligence Data Sources (Free)
type ASNIntelligence struct {
	ASN          string `json:"asn"`
	Name         string `json:"name"`
	Description  string `json:"description"`
	CountryCode  string `json:"country_code"`
	Registry     string `json:"registry"`
	Organization string `json:"organization"`
	IPRanges     []string `json:"ip_ranges"`
}

// BGP Data from RouteViews (Free)
type BGPRoute struct {
	Prefix      string
	ASPath      []string
	Origin      string
	NextHop     string
	Community   []string
}

// ASNEnricher handles ASN-based enrichment
type ASNEnricher struct {
	asnDatabase map[string]ASNIntelligence
	bgpRoutes   map[string]BGPRoute
}

// NewASNEnricher creates enricher using free data sources
func NewASNEnricher() *ASNEnricher {
	return &ASNEnricher{
		asnDatabase: make(map[string]ASNIntelligence),
		bgpRoutes:   make(map[string]BGPRoute),
	}
}

// LoadASNDataFromPeeringDB loads ASN data from PeeringDB (Free API)
func (a *ASNEnricher) LoadASNDataFromPeeringDB() error {
	// PeeringDB API - Free access to ASN information
	url := "https://www.peeringdb.com/api/net"
	
	resp, err := http.Get(url)
	if err != nil {
		return fmt.Errorf("failed to fetch PeeringDB data: %v", err)
	}
	defer resp.Body.Close()
	
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return err
	}
	
	var peeringData struct {
		Data []struct {
			ASN     int    `json:"asn"`
			Name    string `json:"name"`
			Website string `json:"website"`
			Notes   string `json:"notes"`
			Policy  string `json:"policy_general"`
		} `json:"data"`
	}
	
	err = json.Unmarshal(body, &peeringData)
	if err != nil {
		return err
	}
	
	// Process PeeringDB data
	for _, net := range peeringData.Data {
		asnStr := fmt.Sprintf("AS%d", net.ASN)
		a.asnDatabase[asnStr] = ASNIntelligence{
			ASN:         asnStr,
			Name:        net.Name,
			Description: net.Notes,
			Organization: net.Name,
		}
	}
	
	fmt.Printf("Loaded %d ASN records from PeeringDB\n", len(a.asnDatabase))
	return nil
}

// LoadRIRData loads Regional Internet Registry data (Free)
func (a *ASNEnricher) LoadRIRData() error {
	// Download RIPE, ARIN, APNIC, LACNIC, AFRINIC data
	rirSources := map[string]string{
		"RIPE":    "https://ftp.ripe.net/ripe/stats/delegated-ripencc-extended-latest",
		"ARIN":    "https://ftp.arin.net/pub/stats/arin/delegated-arin-extended-latest",
		"APNIC":   "https://ftp.apnic.net/stats/apnic/delegated-apnic-extended-latest",
		"LACNIC":  "https://ftp.lacnic.net/pub/stats/lacnic/delegated-lacnic-extended-latest",
		"AFRINIC": "https://ftp.afrinic.net/pub/stats/afrinic/delegated-afrinic-extended-latest",
	}
	
	for registry, url := range rirSources {
		err := a.loadRIRFile(registry, url)
		if err != nil {
			fmt.Printf("Warning: Failed to load %s data: %v\n", registry, err)
			continue
		}
		fmt.Printf("Loaded %s registry data\n", registry)
	}
	
	return nil
}

func (a *ASNEnricher) loadRIRFile(registry, url string) error {
	resp, err := http.Get(url)
	if err != nil {
		return err
	}
	defer resp.Body.Close()
	
	scanner := bufio.NewScanner(resp.Body)
	for scanner.Scan() {
		line := scanner.Text()
		if strings.HasPrefix(line, "#") {
			continue // Skip comments
		}
		
		// Parse RIR format: registry|cc|type|start|value|date|status|extensions
		parts := strings.Split(line, "|")
		if len(parts) < 7 {
			continue
		}
		
		resourceType := parts[2]
		if resourceType == "asn" {
			asn := "AS" + parts[3]
			countryCode := parts[1]
			
			if existing, exists := a.asnDatabase[asn]; exists {
				existing.CountryCode = countryCode
				existing.Registry = registry
				a.asnDatabase[asn] = existing
			} else {
				a.asnDatabase[asn] = ASNIntelligence{
					ASN:         asn,
					CountryCode: countryCode,
					Registry:    registry,
				}
			}
		}
	}
	
	return scanner.Err()
}

// LoadBGPData loads BGP routing data from RouteViews (Free)
func (a *ASNEnricher) LoadBGPData() error {
	// RouteViews provides free BGP data
	// Note: This is simplified - actual BGP data parsing is more complex
	url := "http://archive.routeviews.org/bgpdata/"
	
	// In practice, you'd download MRT files and parse them
	// For this example, we'll simulate with a simplified approach
	fmt.Println("Loading BGP data from RouteViews...")
	
	// Simulate loading BGP prefixes for demonstration
	samplePrefixes := []string{
		"8.8.8.0/24",
		"1.1.1.0/24", 
		"208.67.222.0/24",
	}
	
	for _, prefix := range samplePrefixes {
		a.bgpRoutes[prefix] = BGPRoute{
			Prefix:  prefix,
			ASPath:  []string{"15169", "13335"},
			Origin:  "IGP",
			NextHop: "192.168.1.1",
		}
	}
	
	return nil
}

// EnrichIPWithASN enriches IP with ASN information
func (a *ASNEnricher) EnrichIPWithASN(ipAddress string) *ASNIntelligence {
	ip := net.ParseIP(ipAddress)
	if ip == nil {
		return nil
	}
	
	// Find matching BGP prefix
	for prefix, route := range a.bgpRoutes {
		_, cidr, err := net.ParseCIDR(prefix)
		if err != nil {
			continue
		}
		
		if cidr.Contains(ip) {
			// Get ASN information from the origin AS
			if len(route.ASPath) > 0 {
				originAS := "AS" + route.ASPath[len(route.ASPath)-1]
				if asnInfo, exists := a.asnDatabase[originAS]; exists {
					return &asnInfo
				}
			}
		}
	}
	
	return nil
}

// Free WHOIS Integration
func (a *ASNEnricher) EnrichWithWHOIS(ipAddress string) (*ASNIntelligence, error) {
	// Use free WHOIS services
	whoisServers := []string{
		"whois.radb.net",
		"whois.ripe.net", 
		"whois.arin.net",
	}
	
	for _, server := range whoisServers {
		// In practice, you'd implement WHOIS protocol client
		// For demonstration, we'll use a simplified approach
		fmt.Printf("Querying WHOIS server %s for IP %s\n", server, ipAddress)
		
		// This would be actual WHOIS query implementation
		// conn, err := net.Dial("tcp", server+":43")
		// ...WHOIS protocol implementation...
	}
	
	return nil, nil
}

// Example usage for enriching ThreatEvents with ASN intelligence
func EnrichThreatEventsWithASN() {
	enricher := NewASNEnricher()
	
	// Load free data sources
	fmt.Println("Loading ASN intelligence from free sources...")
	
	err := enricher.LoadASNDataFromPeeringDB()
	if err != nil {
		fmt.Printf("Error loading PeeringDB data: %v\n", err)
	}
	
	err = enricher.LoadRIRData()
	if err != nil {
		fmt.Printf("Error loading RIR data: %v\n", err)
	}
	
	err = enricher.LoadBGPData()
	if err != nil {
		fmt.Printf("Error loading BGP data: %v\n", err)
	}
	
	// Example: Enrich a sample IP
	testIP := "8.8.8.8"
	asnInfo := enricher.EnrichIPWithASN(testIP)
	if asnInfo != nil {
		fmt.Printf("IP %s belongs to ASN %s (%s)\n", 
			testIP, asnInfo.ASN, asnInfo.Organization)
		
		// Update ThreatEvents table
		updateSQL := `
			UPDATE "ThreatEvents" 
			SET "SourceOrganization" = $1, "SourceISP" = $2
			WHERE "SourceAddress" = $3`
		
		// Execute: db.Exec(updateSQL, asnInfo.Organization, asnInfo.Name, testIP)
	}
}

// Batch processing for large IP datasets
func BatchEnrichASNData(ipAddresses []string) {
	enricher := NewASNEnricher()
	
	// Load all data sources first
	enricher.LoadASNDataFromPeeringDB()
	enricher.LoadRIRData()
	enricher.LoadBGPData()
	
	// Process IPs in batches
	batchSize := 1000
	for i := 0; i < len(ipAddresses); i += batchSize {
		end := i + batchSize
		if end > len(ipAddresses) {
			end = len(ipAddresses)
		}
		
		batch := ipAddresses[i:end]
		fmt.Printf("Processing batch %d-%d of %d IPs\n", i, end, len(ipAddresses))
		
		for _, ip := range batch {
			asnInfo := enricher.EnrichIPWithASN(ip)
			if asnInfo != nil {
				// Process and store ASN information
				fmt.Printf("Enriched IP %s with ASN %s\n", ip, asnInfo.ASN)
			}
		}
	}
}
