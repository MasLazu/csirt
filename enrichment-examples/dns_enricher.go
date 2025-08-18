package main

import (
	"encoding/json"
	"fmt"
	"io"
	"net"
	"net/http"
	"net/url"
	"strings"
	"time"
)

// DNS Intelligence Data Sources
type DNSIntelligence struct {
	Domain           string    `json:"domain"`
	IPAddresses      []string  `json:"ip_addresses"`
	MXRecords        []string  `json:"mx_records"`
	NSRecords        []string  `json:"ns_records"`
	TXTRecords       []string  `json:"txt_records"`
	CreationDate     time.Time `json:"creation_date"`
	ExpirationDate   time.Time `json:"expiration_date"`
	Registrar        string    `json:"registrar"`
	DomainAge        int       `json:"domain_age_days"`
	IsSuspicious     bool      `json:"is_suspicious"`
	ThreatCategories []string  `json:"threat_categories"`
}

// Certificate Transparency Data
type CTLogEntry struct {
	Domain      string    `json:"domain"`
	Issuer      string    `json:"issuer"`
	NotBefore   time.Time `json:"not_before"`
	NotAfter    time.Time `json:"not_after"`
	SerialNumber string   `json:"serial_number"`
	SubjectAltNames []string `json:"subject_alt_names"`
}

// DNSEnricher handles domain and DNS-based enrichment
type DNSEnricher struct {
	httpClient *http.Client
}

// NewDNSEnricher creates a new DNS enricher
func NewDNSEnricher() *DNSEnricher {
	return &DNSEnricher{
		httpClient: &http.Client{
			Timeout: 30 * time.Second,
		},
	}
}

// EnrichWithCertificateTransparency uses free CT logs for domain intelligence
func (d *DNSEnricher) EnrichWithCertificateTransparency(domain string) ([]CTLogEntry, error) {
	// Use crt.sh (free Certificate Transparency search)
	baseURL := "https://crt.sh/"
	params := url.Values{}
	params.Add("q", domain)
	params.Add("output", "json")
	
	resp, err := d.httpClient.Get(baseURL + "?" + params.Encode())
	if err != nil {
		return nil, fmt.Errorf("failed to query crt.sh: %v", err)
	}
	defer resp.Body.Close()
	
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}
	
	var ctEntries []struct {
		IssuerName    string `json:"issuer_name"`
		CommonName    string `json:"common_name"`
		NameValue     string `json:"name_value"`
		NotBefore     string `json:"not_before"`
		NotAfter      string `json:"not_after"`
		SerialNumber  string `json:"serial_number"`
	}
	
	err = json.Unmarshal(body, &ctEntries)
	if err != nil {
		return nil, err
	}
	
	var results []CTLogEntry
	for _, entry := range ctEntries {
		notBefore, _ := time.Parse("2006-01-02T15:04:05", entry.NotBefore)
		notAfter, _ := time.Parse("2006-01-02T15:04:05", entry.NotAfter)
		
		results = append(results, CTLogEntry{
			Domain:       entry.CommonName,
			Issuer:       entry.IssuerName,
			NotBefore:    notBefore,
			NotAfter:     notAfter,
			SerialNumber: entry.SerialNumber,
			SubjectAltNames: strings.Split(entry.NameValue, "\n"),
		})
	}
	
	return results, nil
}

// EnrichWithPassiveDNS uses free passive DNS sources
func (d *DNSEnricher) EnrichWithPassiveDNS(domain string) (*DNSIntelligence, error) {
	// Use SecurityTrails free tier (50 queries/month)
	// Alternative: VirusTotal (free tier has DNS resolution data)
	
	intel := &DNSIntelligence{
		Domain: domain,
	}
	
	// Get current DNS records
	ips, err := net.LookupIP(domain)
	if err == nil {
		for _, ip := range ips {
			intel.IPAddresses = append(intel.IPAddresses, ip.String())
		}
	}
	
	// Get MX records
	mxRecords, err := net.LookupMX(domain)
	if err == nil {
		for _, mx := range mxRecords {
			intel.MXRecords = append(intel.MXRecords, mx.Host)
		}
	}
	
	// Get NS records
	nsRecords, err := net.LookupNS(domain)
	if err == nil {
		for _, ns := range nsRecords {
			intel.NSRecords = append(intel.NSRecords, ns.Host)
		}
	}
	
	// Get TXT records
	txtRecords, err := net.LookupTXT(domain)
	if err == nil {
		intel.TXTRecords = txtRecords
	}
	
	return intel, nil
}

// EnrichWithURLVoid uses URLVoid free API for domain reputation
func (d *DNSEnricher) EnrichWithURLVoid(domain string) (*DNSIntelligence, error) {
	// URLVoid offers free API with 1000 queries/month
	// Sign up at: https://www.urlvoid.com/api/
	
	apiKey := "YOUR_URLVOID_API_KEY" // Get from URLVoid
	apiURL := fmt.Sprintf("https://api.urlvoid.com/v1/pay-as-you-go/?key=%s&host=%s", 
		apiKey, url.QueryEscape(domain))
	
	resp, err := d.httpClient.Get(apiURL)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()
	
	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}
	
	var urlvoidResp struct {
		Success bool `json:"success"`
		Data    struct {
			Report struct {
				Domain       string `json:"domain"`
				IPAddress    string `json:"ip_address"`
				Country      string `json:"country"`
				CountryCode  string `json:"country_code"`
				Detections   int    `json:"detections"`
				Engines      int    `json:"engines"`
				ScanDate     string `json:"scan_date"`
			} `json:"report"`
		} `json:"data"`
	}
	
	err = json.Unmarshal(body, &urlvoidResp)
	if err != nil {
		return nil, err
	}
	
	intel := &DNSIntelligence{
		Domain:      domain,
		IPAddresses: []string{urlvoidResp.Data.Report.IPAddress},
		IsSuspicious: urlvoidResp.Data.Report.Detections > 0,
	}
	
	if urlvoidResp.Data.Report.Detections > 0 {
		intel.ThreatCategories = append(intel.ThreatCategories, "malicious")
	}
	
	return intel, nil
}

// AnalyzeDomainPatterns analyzes domains for suspicious patterns
func (d *DNSEnricher) AnalyzeDomainPatterns(domain string) map[string]interface{} {
	analysis := make(map[string]interface{})
	
	// Domain length analysis
	analysis["domain_length"] = len(domain)
	
	// Subdomain count
	parts := strings.Split(domain, ".")
	analysis["subdomain_count"] = len(parts) - 2 // Exclude domain and TLD
	
	// Character analysis
	hasNumbers := false
	hasHyphens := false
	consecutiveConsonants := 0
	
	for _, char := range domain {
		if char >= '0' && char <= '9' {
			hasNumbers = true
		}
		if char == '-' {
			hasHyphens = true
		}
	}
	
	analysis["has_numbers"] = hasNumbers
	analysis["has_hyphens"] = hasHyphens
	analysis["consecutive_consonants"] = consecutiveConsonants
	
	// Suspicious patterns
	suspiciousPatterns := []string{
		"secure", "login", "verify", "update", "account", 
		"paypal", "amazon", "microsoft", "google", "apple",
	}
	
	suspiciousScore := 0
	for _, pattern := range suspiciousPatterns {
		if strings.Contains(strings.ToLower(domain), pattern) {
			suspiciousScore++
		}
	}
	analysis["suspicious_score"] = suspiciousScore
	
	// DGA (Domain Generation Algorithm) indicators
	dgaScore := 0
	if len(domain) > 15 {
		dgaScore += 2
	}
	if hasNumbers && hasHyphens {
		dgaScore += 3
	}
	if analysis["subdomain_count"].(int) > 3 {
		dgaScore += 2
	}
	
	analysis["dga_score"] = dgaScore
	analysis["likely_dga"] = dgaScore > 5
	
	return analysis
}

// EnrichThreatEventsWithDNS enriches threat events with DNS intelligence
func EnrichThreatEventsWithDNS() {
	enricher := NewDNSEnricher()
	
	// Example domains from your threat events (you'd get these from database)
	testDomains := []string{
		"malicious-example.com",
		"suspicious-domain.net",
		"phishing-site.org",
	}
	
	for _, domain := range testDomains {
		fmt.Printf("\nAnalyzing domain: %s\n", domain)
		
		// Get certificate transparency data
		ctData, err := enricher.EnrichWithCertificateTransparency(domain)
		if err != nil {
			fmt.Printf("CT lookup failed: %v\n", err)
		} else {
			fmt.Printf("Found %d certificate entries\n", len(ctData))
		}
		
		// Get passive DNS data
		dnsIntel, err := enricher.EnrichWithPassiveDNS(domain)
		if err != nil {
			fmt.Printf("DNS lookup failed: %v\n", err)
		} else {
			fmt.Printf("Resolved to IPs: %v\n", dnsIntel.IPAddresses)
		}
		
		// Analyze domain patterns
		patterns := enricher.AnalyzeDomainPatterns(domain)
		fmt.Printf("Domain analysis: %+v\n", patterns)
		
		// Store enrichment data
		updateSQL := `
			UPDATE "ThreatEvents" 
			SET "DomainAge" = $1, "IsSuspiciousDomain" = $2, "DGAScore" = $3
			WHERE "DestinationAddress" IN (
				SELECT UNNEST(string_to_array($4, ','))::inet
			)`
		
		// Execute with your database connection
		// ips := strings.Join(dnsIntel.IPAddresses, ",")
		// db.Exec(updateSQL, patterns["domain_length"], patterns["likely_dga"], patterns["dga_score"], ips)
	}
}

// Free Domain Intelligence Sources Summary:
func GetFreeDomainIntelligenceSources() map[string]interface{} {
	return map[string]interface{}{
		"certificate_transparency": map[string]interface{}{
			"crt.sh":           "https://crt.sh/ - Free CT log search",
			"censys":          "https://censys.io/ - Free tier: 1000 queries/month",
			"shodan":          "https://www.shodan.io/ - Free tier: 100 queries/month",
		},
		"domain_reputation": map[string]interface{}{
			"urlvoid":         "https://www.urlvoid.com/ - Free tier: 1000 queries/month",
			"virustotal":      "https://www.virustotal.com/ - Free tier: 4 req/min",
			"hybrid_analysis": "https://www.hybrid-analysis.com/ - Free tier available",
		},
		"passive_dns": map[string]interface{}{
			"securitytrails":  "https://securitytrails.com/ - Free tier: 50 queries/month",
			"circl_pdns":      "https://www.circl.lu/services/passive-dns/ - Free for research",
			"dnsdb":           "https://www.farsightsecurity.com/ - Commercial but has free research access",
		},
		"whois_data": map[string]interface{}{
			"whois_api":       "https://whoisapi.net/ - Free tier: 500 queries/month",
			"jsonwhois":       "https://jsonwhois.com/ - Free tier available",
			"whoxy":           "https://www.whoxy.com/ - Free tier: 500 queries/month",
		},
	}
}
