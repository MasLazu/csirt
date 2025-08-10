package main

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"net"
	"strings"
	"sync"
	"time"
	"unicode/utf8"

	"github.com/google/uuid"
	_ "github.com/lib/pq"
)

// PostgreSQLClient wraps PostgreSQL operations
type PostgreSQLClient struct {
	db     *sql.DB
	config PostgreSQLConfig

	// Prepared statements for batch inserts
	insertAsnStmt           *sql.Stmt
	insertCountryStmt       *sql.Stmt
	insertProtocolStmt      *sql.Stmt
	insertMalwareFamilyStmt *sql.Stmt
	insertThreatStmt        *sql.Stmt

	// Lookup caches for normalized data
	asnCache           map[string]int
	countryCache       map[string]int
	protocolCache      map[string]int
	malwareFamilyCache map[string]int

	// Mutex for thread-safe cache access
	cacheMutex sync.RWMutex
}

// MigrationProgress represents the current migration state
type MigrationProgress struct {
	LastProcessedID string    `json:"last_processed_id"`
	ProcessedCount  int64     `json:"processed_count"`
	TotalCount      int64     `json:"total_count"`
	StartTime       time.Time `json:"start_time"`
	LastUpdateTime  time.Time `json:"last_update_time"`
}

// ThreatRecord represents the normalized threat intelligence record for PostgreSQL
type ThreatRecord struct {
	ID                   uuid.UUID
	Timestamp            time.Time
	AsnID                int
	SourceAddress        net.IP
	SourceCountryID      *int
	DestinationAddress   *net.IP
	DestinationCountryID *int
	SourcePort           *int
	DestinationPort      *int
	ProtocolID           *int
	Category             string
	MalwareFamilyID      *int
	CreatedAt            time.Time
	UpdatedAt            time.Time
}

// NewPostgreSQLClient creates a new PostgreSQL client
func NewPostgreSQLClient(config PostgreSQLConfig, migrationConfig MigrationConfig) (*PostgreSQLClient, error) {
	connStr := config.ConnectionString()
	log.Printf("Connecting to PostgreSQL with connection string: %s", connStr)
	db, err := sql.Open("postgres", connStr)
	if err != nil {
		return nil, fmt.Errorf("failed to open PostgreSQL connection: %w", err)
	}

	// Configure connection pool
	db.SetMaxOpenConns(migrationConfig.ConnectionPoolSize)
	db.SetMaxIdleConns(migrationConfig.ConnectionPoolSize / 2)
	db.SetConnMaxLifetime(time.Hour)

	log.Println("Testing PostgreSQL connection...")
	// Test the connection
	if err := db.Ping(); err != nil {
		return nil, fmt.Errorf("failed to ping PostgreSQL: %w", err)
	}
	log.Println("PostgreSQL connection successful")

	client := &PostgreSQLClient{
		db:                 db,
		config:             config,
		asnCache:           make(map[string]int),
		countryCache:       make(map[string]int),
		protocolCache:      make(map[string]int),
		malwareFamilyCache: make(map[string]int),
	}

	// Prepare statements
	log.Println("Preparing SQL statements...")
	if err := client.prepareStatements(); err != nil {
		return nil, fmt.Errorf("failed to prepare statements: %w", err)
	}
	log.Println("SQL statements prepared successfully")

	// Load existing lookup data into caches
	log.Println("Loading lookup caches...")
	if err := client.loadLookupCaches(); err != nil {
		return nil, fmt.Errorf("failed to load lookup caches: %w", err)
	}

	log.Printf("Connected to PostgreSQL: %s:%d/%s", config.Host, config.Port, config.Database)

	// Progress tracking is now based on existing record count in PostgreSQL

	return client, nil
}

// Close closes the PostgreSQL connection and prepared statements
func (p *PostgreSQLClient) Close() error {
	// Close prepared statements
	statements := []*sql.Stmt{
		p.insertAsnStmt,
		p.insertCountryStmt,
		p.insertProtocolStmt,
		p.insertMalwareFamilyStmt,
		p.insertThreatStmt,
	}

	for _, stmt := range statements {
		if stmt != nil {
			stmt.Close()
		}
	}

	return p.db.Close()
}

// prepareStatements prepares all SQL statements for batch operations
func (p *PostgreSQLClient) prepareStatements() error {
	var err error

	// ASN info insert statement
	p.insertAsnStmt, err = p.db.Prepare(`
		INSERT INTO asn_info (asn, description, created_at) 
		VALUES ($1, $2, $3) 
		ON CONFLICT (asn) DO NOTHING 
		RETURNING id`)
	if err != nil {
		return fmt.Errorf("failed to prepare ASN insert statement: %w", err)
	}

	// Country insert statement
	p.insertCountryStmt, err = p.db.Prepare(`
		INSERT INTO countries (code, name, created_at) 
		VALUES ($1, $2, $3) 
		ON CONFLICT (code) DO NOTHING 
		RETURNING id`)
	if err != nil {
		return fmt.Errorf("failed to prepare country insert statement: %w", err)
	}

	// Protocol insert statement
	p.insertProtocolStmt, err = p.db.Prepare(`
		INSERT INTO protocols (name, created_at) 
		VALUES ($1, $2) 
		ON CONFLICT (name) DO NOTHING 
		RETURNING id`)
	if err != nil {
		return fmt.Errorf("failed to prepare protocol insert statement: %w", err)
	}

	// Malware family insert statement
	p.insertMalwareFamilyStmt, err = p.db.Prepare(`
		INSERT INTO malware_families (name, created_at) 
		VALUES ($1, $2) 
		ON CONFLICT (name) DO NOTHING 
		RETURNING id`)
	if err != nil {
		return fmt.Errorf("failed to prepare malware family insert statement: %w", err)
	}

	// Threat intelligence insert statement
	p.insertThreatStmt, err = p.db.Prepare(`
		INSERT INTO threat_intelligence (
			id, timestamp, asn_id, source_address, source_country_id,
			destination_address, destination_country_id, source_port, destination_port,
			protocol_id, category, malware_family_id, created_at, updated_at
		) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12, $13, $14)`)
	if err != nil {
		return fmt.Errorf("failed to prepare threat insert statement: %w", err)
	}

	return nil
}

// loadLookupCaches loads existing lookup data into memory caches
func (p *PostgreSQLClient) loadLookupCaches() error {
	log.Println("Loading ASN cache...")
	// Load ASN cache with timeout
	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	rows, err := p.db.QueryContext(ctx, "SELECT id, asn FROM asn_info LIMIT 10000")
	if err != nil {
		// If table doesn't exist, that's okay - we'll create entries as needed
		log.Printf("Warning: Could not load ASN cache (table may not exist): %v", err)
	} else {
		defer rows.Close()
		asnCount := 0
		for rows.Next() {
			var id int
			var asn string
			if err := rows.Scan(&id, &asn); err != nil {
				log.Printf("Warning: failed to scan ASN row: %v", err)
				continue
			}
			p.asnCache[asn] = id
			asnCount++
		}
		log.Printf("Loaded %d ASN entries", asnCount)
	}

	log.Println("Loading country cache...")
	// Load country cache with timeout
	ctx2, cancel2 := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel2()

	rows, err = p.db.QueryContext(ctx2, "SELECT id, code FROM countries LIMIT 1000")
	if err != nil {
		log.Printf("Warning: Could not load country cache (table may not exist): %v", err)
	} else {
		defer rows.Close()
		countryCount := 0
		for rows.Next() {
			var id int
			var code string
			if err := rows.Scan(&id, &code); err != nil {
				log.Printf("Warning: failed to scan country row: %v", err)
				continue
			}
			p.countryCache[code] = id
			countryCount++
		}
		log.Printf("Loaded %d country entries", countryCount)
	}

	log.Println("Loading protocol cache...")
	// Load protocol cache with timeout
	ctx3, cancel3 := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel3()

	rows, err = p.db.QueryContext(ctx3, "SELECT id, name FROM protocols LIMIT 1000")
	if err != nil {
		log.Printf("Warning: Could not load protocol cache (table may not exist): %v", err)
	} else {
		defer rows.Close()
		protocolCount := 0
		for rows.Next() {
			var id int
			var name string
			if err := rows.Scan(&id, &name); err != nil {
				log.Printf("Warning: failed to scan protocol row: %v", err)
				continue
			}
			p.protocolCache[name] = id
			protocolCount++
		}
		log.Printf("Loaded %d protocol entries", protocolCount)
	}

	log.Println("Loading malware family cache...")
	// Load malware family cache with timeout
	ctx4, cancel4 := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel4()

	rows, err = p.db.QueryContext(ctx4, "SELECT id, name FROM malware_families LIMIT 1000")
	if err != nil {
		log.Printf("Warning: Could not load malware family cache (table may not exist): %v", err)
	} else {
		defer rows.Close()
		familyCount := 0
		for rows.Next() {
			var id int
			var name string
			if err := rows.Scan(&id, &name); err != nil {
				log.Printf("Warning: failed to scan malware family row: %v", err)
				continue
			}
			p.malwareFamilyCache[name] = id
			familyCount++
		}
		log.Printf("Loaded %d malware family entries", familyCount)
	}

	log.Printf("Loaded lookup caches: %d ASNs, %d countries, %d protocols, %d malware families",
		len(p.asnCache), len(p.countryCache), len(p.protocolCache), len(p.malwareFamilyCache))

	return nil
}

// GetOrCreateAsnID gets or creates an ASN record and returns its ID
func (p *PostgreSQLClient) GetOrCreateAsnID(asn, description string) (int, error) {
	// Sanitize inputs to prevent UTF-8 issues
	asn = sanitizeUTF8String(asn)
	description = sanitizeUTF8String(description)

	// Thread-safe cache access
	p.cacheMutex.RLock()
	if id, exists := p.asnCache[asn]; exists {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	var id int
	err := p.insertAsnStmt.QueryRow(asn, description, time.Now()).Scan(&id)
	if err != nil {
		// If no rows returned, the record already exists, query for it
		if err == sql.ErrNoRows {
			err = p.db.QueryRow("SELECT id FROM asn_info WHERE asn = $1", asn).Scan(&id)
			if err != nil {
				return 0, fmt.Errorf("failed to get existing ASN ID: %w", err)
			}
		} else {
			return 0, fmt.Errorf("failed to insert ASN: %w", err)
		}
	}

	// Thread-safe cache update
	p.cacheMutex.Lock()
	p.asnCache[asn] = id
	p.cacheMutex.Unlock()

	return id, nil
}

// GetOrCreateCountryID gets or creates a country record and returns its ID
func (p *PostgreSQLClient) GetOrCreateCountryID(code, name string) (int, error) {
	// Sanitize inputs to prevent UTF-8 issues
	code = sanitizeUTF8String(code)
	name = sanitizeUTF8String(name)

	// Thread-safe cache access
	p.cacheMutex.RLock()
	if id, exists := p.countryCache[code]; exists {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	var id int
	err := p.insertCountryStmt.QueryRow(code, name, time.Now()).Scan(&id)
	if err != nil {
		if err == sql.ErrNoRows {
			err = p.db.QueryRow("SELECT id FROM countries WHERE code = $1", code).Scan(&id)
			if err != nil {
				return 0, fmt.Errorf("failed to get existing country ID: %w", err)
			}
		} else {
			return 0, fmt.Errorf("failed to insert country: %w", err)
		}
	}

	// Thread-safe cache update
	p.cacheMutex.Lock()
	p.countryCache[code] = id
	p.cacheMutex.Unlock()

	return id, nil
}

// GetOrCreateProtocolID gets or creates a protocol record and returns its ID
func (p *PostgreSQLClient) GetOrCreateProtocolID(name string) (int, error) {
	// Sanitize input to prevent UTF-8 issues
	name = sanitizeUTF8String(name)

	// Thread-safe cache access
	p.cacheMutex.RLock()
	if id, exists := p.protocolCache[name]; exists {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	var id int
	err := p.insertProtocolStmt.QueryRow(name, time.Now()).Scan(&id)
	if err != nil {
		if err == sql.ErrNoRows {
			err = p.db.QueryRow("SELECT id FROM protocols WHERE name = $1", name).Scan(&id)
			if err != nil {
				return 0, fmt.Errorf("failed to get existing protocol ID: %w", err)
			}
		} else {
			return 0, fmt.Errorf("failed to insert protocol: %w", err)
		}
	}

	// Thread-safe cache update
	p.cacheMutex.Lock()
	p.protocolCache[name] = id
	p.cacheMutex.Unlock()

	return id, nil
}

// GetOrCreateMalwareFamilyID gets or creates a malware family record and returns its ID
func (p *PostgreSQLClient) GetOrCreateMalwareFamilyID(name string) (int, error) {
	// Sanitize input to prevent UTF-8 issues
	name = sanitizeUTF8String(name)

	// Thread-safe cache access
	p.cacheMutex.RLock()
	if id, exists := p.malwareFamilyCache[name]; exists {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	var id int
	err := p.insertMalwareFamilyStmt.QueryRow(name, time.Now()).Scan(&id)
	if err != nil {
		if err == sql.ErrNoRows {
			err = p.db.QueryRow("SELECT id FROM malware_families WHERE name = $1", name).Scan(&id)
			if err != nil {
				return 0, fmt.Errorf("failed to get existing malware family ID: %w", err)
			}
		} else {
			return 0, fmt.Errorf("failed to insert malware family: %w", err)
		}
	}

	// Thread-safe cache update
	p.cacheMutex.Lock()
	p.malwareFamilyCache[name] = id
	p.cacheMutex.Unlock()

	return id, nil
}

// InsertThreatBatch inserts a batch of threat records
func (p *PostgreSQLClient) InsertThreatBatch(threats []ThreatRecord) error {
	tx, err := p.db.Begin()
	if err != nil {
		return fmt.Errorf("failed to begin transaction: %w", err)
	}
	defer tx.Rollback()

	stmt := tx.Stmt(p.insertThreatStmt)
	defer stmt.Close()

	for _, threat := range threats {
		// Final sanitization of Category field before database insert
		sanitizedCategory := sanitizeUTF8String(threat.Category)

		// Convert IP addresses to strings and sanitize them
		sourceAddrStr := threat.SourceAddress.String()
		sourceAddrStr = sanitizeUTF8String(sourceAddrStr)

		var destAddrStr *string
		if threat.DestinationAddress != nil {
			destAddr := threat.DestinationAddress.String()
			destAddr = sanitizeUTF8String(destAddr)
			destAddrStr = &destAddr
		}

		_, err := stmt.Exec(
			threat.ID,
			threat.Timestamp,
			threat.AsnID,
			sourceAddrStr,
			threat.SourceCountryID,
			destAddrStr,
			threat.DestinationCountryID,
			threat.SourcePort,
			threat.DestinationPort,
			threat.ProtocolID,
			sanitizedCategory,
			threat.MalwareFamilyID,
			threat.CreatedAt,
			threat.UpdatedAt,
		)
		if err != nil {
			return fmt.Errorf("failed to insert threat record %s: %w", threat.ID, err)
		}
	}

	if err := tx.Commit(); err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}

	return nil
}

// parseAndValidateIPAddress parses and validates an IP address string
func (p *PostgreSQLClient) parseAndValidateIPAddress(ipStr string) (net.IP, error) {
	if ipStr == "" {
		return nil, fmt.Errorf("IP address string is empty")
	}

	// Trim whitespace
	ipStr = strings.TrimSpace(ipStr)
	if ipStr == "" {
		return nil, fmt.Errorf("IP address string is empty after trimming")
	}

	ip := net.ParseIP(ipStr)
	if ip == nil {
		return nil, fmt.Errorf("invalid IP address format: %s", ipStr)
	}

	// Additional validation for common invalid patterns
	if ip.IsUnspecified() {
		return nil, fmt.Errorf("unspecified IP address (0.0.0.0 or ::): %s", ipStr)
	}

	return ip, nil
}

// parseAndValidatePort parses and validates a port string
func (p *PostgreSQLClient) parseAndValidatePort(portStr string) (int, error) {
	if portStr == "" {
		return 0, fmt.Errorf("port string is empty")
	}

	port := 0
	_, err := fmt.Sscanf(portStr, "%d", &port)
	if err != nil {
		return 0, fmt.Errorf("invalid port format: %w", err)
	}

	if port < 1 || port > 65535 {
		return 0, fmt.Errorf("port %d is out of valid range (1-65535)", port)
	}

	return port, nil
}

// normalizeASN normalizes ASN data by removing common prefixes and cleaning format
func (p *PostgreSQLClient) normalizeASN(asn string) string {
	if asn == "" {
		return ""
	}

	// Trim whitespace first
	normalized := strings.TrimSpace(asn)
	if normalized == "" {
		return ""
	}

	// Convert to uppercase for consistent processing
	normalized = strings.ToUpper(normalized)

	// Remove common prefixes like "AS" or "ASN"
	if strings.HasPrefix(normalized, "AS") {
		normalized = normalized[2:]
	} else if strings.HasPrefix(normalized, "ASN") {
		normalized = normalized[3:]
	}

	// Remove any remaining non-numeric characters except for valid ASN formats
	normalized = strings.TrimSpace(normalized)
	if normalized == "" {
		return asn // Return original if we can't normalize
	}

	// Validate that we have a numeric ASN
	var asnNumber int
	if _, err := fmt.Sscanf(normalized, "%d", &asnNumber); err != nil {
		// If not a simple number, try to extract the first number
		for i, char := range normalized {
			if char < '0' || char > '9' {
				if i > 0 {
					normalized = normalized[:i]
					break
				} else {
					return asn // Return original if no valid number found
				}
			}
		}
	}

	// Validate ASN range (1-4294967295 for 32-bit ASNs)
	if asnNumber < 1 || asnNumber > 4294967295 {
		return asn // Return original if out of valid range
	}

	// Add AS prefix for consistency
	return fmt.Sprintf("AS%s", normalized)
}

// normalizeASNInfo normalizes ASN description information
func (p *PostgreSQLClient) normalizeASNInfo(asnInfo string) string {
	if asnInfo == "" {
		return "Unknown ASN"
	}

	// Sanitize string to remove null bytes and other problematic characters
	normalized := sanitizeUTF8String(asnInfo)

	// Trim whitespace
	normalized = strings.TrimSpace(normalized)
	if normalized == "" {
		return "Unknown ASN"
	}

	// Remove common unwanted characters and clean up
	normalized = strings.ReplaceAll(normalized, "\n", " ")
	normalized = strings.ReplaceAll(normalized, "\r", " ")
	normalized = strings.ReplaceAll(normalized, "\t", " ")

	// Collapse multiple spaces into single space
	for strings.Contains(normalized, "  ") {
		normalized = strings.ReplaceAll(normalized, "  ", " ")
	}

	// Limit length to match database constraint (assuming 255 chars)
	if len(normalized) > 255 {
		normalized = normalized[:252] + "..."
	}

	return normalized
}

// normalizeCategory normalizes threat category data
func (p *PostgreSQLClient) normalizeCategory(category string) string {
	if category == "" {
		return ""
	}

	// Sanitize string first
	category = sanitizeUTF8String(category)

	// Trim whitespace and convert to lowercase for consistency
	normalized := strings.TrimSpace(strings.ToLower(category))
	if normalized == "" {
		return ""
	}

	// Remove common unwanted characters
	normalized = strings.ReplaceAll(normalized, "\n", " ")
	normalized = strings.ReplaceAll(normalized, "\r", " ")
	normalized = strings.ReplaceAll(normalized, "\t", " ")

	// Collapse multiple spaces into single space
	for strings.Contains(normalized, "  ") {
		normalized = strings.ReplaceAll(normalized, "  ", " ")
	}

	// Map common category variations to standard names
	categoryMap := map[string]string{
		"malware":     "malware",
		"botnet":      "botnet",
		"phishing":    "phishing",
		"spam":        "spam",
		"c&c":         "c2",
		"c2":          "c2",
		"command":     "c2",
		"control":     "c2",
		"exploit":     "exploit",
		"bruteforce":  "brute_force",
		"brute force": "brute_force",
		"ddos":        "ddos",
		"dos":         "dos",
		"scanner":     "scanner",
		"scan":        "scanner",
	}

	// Check for exact matches first
	if standardName, exists := categoryMap[normalized]; exists {
		return standardName
	}

	// Check for partial matches
	for key, value := range categoryMap {
		if strings.Contains(normalized, key) {
			return value
		}
	}

	// Limit length to match database constraint
	if len(normalized) > 50 {
		normalized = normalized[:50]
	}

	return normalized
}

// normalizeCountryCode normalizes country code to ISO 3166-1 alpha-2 format
func (p *PostgreSQLClient) normalizeCountryCode(country string) string {
	if country == "" {
		return ""
	}

	// Trim whitespace and convert to uppercase
	normalized := strings.TrimSpace(strings.ToUpper(country))
	if normalized == "" {
		return ""
	}

	// Handle common country name to code mappings
	countryMap := map[string]string{
		"UNITED STATES":      "US",
		"USA":                "US",
		"AMERICA":            "US",
		"UNITED KINGDOM":     "GB",
		"UK":                 "GB",
		"GREAT BRITAIN":      "GB",
		"CHINA":              "CN",
		"RUSSIA":             "RU",
		"RUSSIAN FEDERATION": "RU",
		"GERMANY":            "DE",
		"FRANCE":             "FR",
		"JAPAN":              "JP",
		"CANADA":             "CA",
		"AUSTRALIA":          "AU",
		"BRAZIL":             "BR",
		"INDIA":              "IN",
		"SOUTH KOREA":        "KR",
		"KOREA":              "KR",
		"NETHERLANDS":        "NL",
		"HOLLAND":            "NL",
		"SPAIN":              "ES",
		"ITALY":              "IT",
		"SWEDEN":             "SE",
		"NORWAY":             "NO",
		"DENMARK":            "DK",
		"FINLAND":            "FI",
		"POLAND":             "PL",
		"UKRAINE":            "UA",
		"TURKEY":             "TR",
		"ISRAEL":             "IL",
		"SOUTH AFRICA":       "ZA",
		"MEXICO":             "MX",
		"ARGENTINA":          "AR",
		"CHILE":              "CL",
		"COLOMBIA":           "CO",
		"VENEZUELA":          "VE",
		"PERU":               "PE",
		"ECUADOR":            "EC",
		"BOLIVIA":            "BO",
		"URUGUAY":            "UY",
		"PARAGUAY":           "PY",
		"THAILAND":           "TH",
		"VIETNAM":            "VN",
		"SINGAPORE":          "SG",
		"MALAYSIA":           "MY",
		"INDONESIA":          "ID",
		"PHILIPPINES":        "PH",
		"TAIWAN":             "TW",
		"HONG KONG":          "HK",
		"NEW ZEALAND":        "NZ",
		"SWITZERLAND":        "CH",
		"AUSTRIA":            "AT",
		"BELGIUM":            "BE",
		"PORTUGAL":           "PT",
		"GREECE":             "GR",
		"CZECH REPUBLIC":     "CZ",
		"HUNGARY":            "HU",
		"ROMANIA":            "RO",
		"BULGARIA":           "BG",
		"CROATIA":            "HR",
		"SERBIA":             "RS",
		"SLOVENIA":           "SI",
		"SLOVAKIA":           "SK",
		"ESTONIA":            "EE",
		"LATVIA":             "LV",
		"LITHUANIA":          "LT",
		"IRELAND":            "IE",
		"ICELAND":            "IS",
		"LUXEMBOURG":         "LU",
		"MALTA":              "MT",
		"CYPRUS":             "CY",
	}

	// Check if it's a full country name that can be mapped
	if code, exists := countryMap[normalized]; exists {
		return code
	}

	// If it's already a 2-character code, validate and return
	if len(normalized) == 2 {
		// Validate it looks like a country code (2 letters)
		for _, char := range normalized {
			if char < 'A' || char > 'Z' {
				return "" // Invalid country code format
			}
		}
		return normalized
	}

	// If it's longer than 2 characters, try to extract a valid 2-char code from the beginning
	if len(normalized) > 2 {
		candidate := normalized[:2]
		// Validate the candidate
		for _, char := range candidate {
			if char < 'A' || char > 'Z' {
				return "" // Invalid country code format
			}
		}
		return candidate
	}

	return ""
}

// normalizeProtocol normalizes protocol names to standard format
func (p *PostgreSQLClient) normalizeProtocol(protocol string) string {
	if protocol == "" {
		return ""
	}

	// Sanitize string first
	protocol = sanitizeUTF8String(protocol)

	// Trim whitespace and convert to uppercase for consistency
	normalized := strings.TrimSpace(strings.ToUpper(protocol))
	if normalized == "" {
		return ""
	}

	// Remove common unwanted characters
	normalized = strings.ReplaceAll(normalized, "\n", "")
	normalized = strings.ReplaceAll(normalized, "\r", "")
	normalized = strings.ReplaceAll(normalized, "\t", "")

	// Map common protocol variations to standard names
	protocolMap := map[string]string{
		"TCP":      "TCP",
		"UDP":      "UDP",
		"ICMP":     "ICMP",
		"HTTP":     "HTTP",
		"HTTPS":    "HTTPS",
		"FTP":      "FTP",
		"SSH":      "SSH",
		"DNS":      "DNS",
		"SMTP":     "SMTP",
		"POP3":     "POP3",
		"IMAP":     "IMAP",
		"TELNET":   "TELNET",
		"SNMP":     "SNMP",
		"DHCP":     "DHCP",
		"TFTP":     "TFTP",
		"NTP":      "NTP",
		"LDAP":     "LDAP",
		"SIP":      "SIP",
		"RTP":      "RTP",
		"RTCP":     "RTCP",
		"H323":     "H323",
		"MGCP":     "MGCP",
		"SCTP":     "SCTP",
		"GRE":      "GRE",
		"ESP":      "ESP",
		"AH":       "AH",
		"OSPF":     "OSPF",
		"BGP":      "BGP",
		"RIP":      "RIP",
		"EIGRP":    "EIGRP",
		"ISIS":     "ISIS",
		"VRRP":     "VRRP",
		"HSRP":     "HSRP",
		"GLBP":     "GLBP",
		"LACP":     "LACP",
		"STP":      "STP",
		"RSTP":     "RSTP",
		"MSTP":     "MSTP",
		"LLDP":     "LLDP",
		"CDP":      "CDP",
		"VTP":      "VTP",
		"DTP":      "DTP",
		"PAGP":     "PAGP",
		"UDLD":     "UDLD",
		"BPDU":     "BPDU",
		"ARP":      "ARP",
		"RARP":     "RARP",
		"BOOTP":    "BOOTP",
		"RADIUS":   "RADIUS",
		"TACACS":   "TACACS",
		"TACACS+":  "TACACS+",
		"KERBEROS": "KERBEROS",
		"IPV6":     "IPV6",
		"ICMPV6":   "ICMPV6",
		"MLDV2":    "MLDV2",
		"IGMP":     "IGMP",
		"PIM":      "PIM",
		"DVMRP":    "DVMRP",
		"MOSPF":    "MOSPF",
	}

	// Check for exact matches first
	if standardName, exists := protocolMap[normalized]; exists {
		return standardName
	}

	// Check for partial matches or common variations
	for key, value := range protocolMap {
		if strings.Contains(normalized, key) {
			return value
		}
	}

	// Handle numeric protocol numbers (convert to string representation)
	var protocolNum int
	if _, err := fmt.Sscanf(normalized, "%d", &protocolNum); err == nil {
		// Map common protocol numbers to names
		protocolNumMap := map[int]string{
			1:   "ICMP",
			6:   "TCP",
			17:  "UDP",
			47:  "GRE",
			50:  "ESP",
			51:  "AH",
			89:  "OSPF",
			132: "SCTP",
		}
		if protocolName, exists := protocolNumMap[protocolNum]; exists {
			return protocolName
		}
		// For unknown protocol numbers, return as PROTO_<number>
		return fmt.Sprintf("PROTO_%d", protocolNum)
	}

	// For unknown protocols, clean and return uppercase version limited to 20 chars
	// Remove any non-alphanumeric characters except underscores and hyphens
	var cleanedProtocol strings.Builder
	for _, char := range normalized {
		if (char >= 'A' && char <= 'Z') || (char >= '0' && char <= '9') || char == '_' || char == '-' {
			cleanedProtocol.WriteRune(char)
		}
	}

	result := cleanedProtocol.String()
	if len(result) > 20 {
		result = result[:20]
	}

	// If result is empty after cleaning, return "UNKNOWN"
	if result == "" {
		return "UNKNOWN"
	}

	return result
}

// normalizeMalwareFamily normalizes malware family names
func (p *PostgreSQLClient) normalizeMalwareFamily(family string) string {
	if family == "" {
		return ""
	}

	// Sanitize string first
	family = sanitizeUTF8String(family)

	// Trim whitespace
	normalized := strings.TrimSpace(family)
	if normalized == "" {
		return ""
	}

	// Remove common unwanted characters and clean up
	normalized = strings.ReplaceAll(normalized, "\n", " ")
	normalized = strings.ReplaceAll(normalized, "\r", " ")
	normalized = strings.ReplaceAll(normalized, "\t", " ")

	// Collapse multiple spaces into single space
	for strings.Contains(normalized, "  ") {
		normalized = strings.ReplaceAll(normalized, "  ", " ")
	}

	// Convert to lowercase for consistency
	normalized = strings.ToLower(normalized)

	// Map common malware family variations to standard names
	malwareFamilyMap := map[string]string{
		"trojan":               "trojan",
		"backdoor":             "backdoor",
		"rootkit":              "rootkit",
		"worm":                 "worm",
		"virus":                "virus",
		"ransomware":           "ransomware",
		"spyware":              "spyware",
		"adware":               "adware",
		"keylogger":            "keylogger",
		"botnet":               "botnet",
		"rat":                  "rat",
		"remote access":        "rat",
		"banking":              "banking_trojan",
		"banker":               "banking_trojan",
		"infostealer":          "infostealer",
		"info stealer":         "infostealer",
		"stealer":              "infostealer",
		"downloader":           "downloader",
		"dropper":              "dropper",
		"loader":               "loader",
		"cryptominer":          "cryptominer",
		"miner":                "cryptominer",
		"coinminer":            "cryptominer",
		"fileless":             "fileless",
		"apt":                  "apt",
		"advanced persistent":  "apt",
		"exploit kit":          "exploit_kit",
		"exploit":              "exploit",
		"zero day":             "zero_day",
		"0day":                 "zero_day",
		"polymorphic":          "polymorphic",
		"metamorphic":          "metamorphic",
		"packed":               "packed",
		"obfuscated":           "obfuscated",
		"webshell":             "webshell",
		"web shell":            "webshell",
		"shell":                "webshell",
		"phishing":             "phishing",
		"scareware":            "scareware",
		"fake av":              "fake_antivirus",
		"fake antivirus":       "fake_antivirus",
		"rogue":                "rogue",
		"pup":                  "pup",
		"potentially unwanted": "pup",
		"greyware":             "greyware",
		"suspicious":           "suspicious",
		"generic":              "generic",
		"heuristic":            "heuristic",
	}

	// Check for exact matches first
	if standardName, exists := malwareFamilyMap[normalized]; exists {
		return standardName
	}

	// Check for partial matches
	for key, value := range malwareFamilyMap {
		if strings.Contains(normalized, key) {
			return value
		}
	}

	// Clean the family name by removing special characters except alphanumeric, spaces, hyphens, and underscores
	var cleanedFamily strings.Builder
	for _, char := range normalized {
		if (char >= 'a' && char <= 'z') || (char >= '0' && char <= '9') || char == ' ' || char == '_' || char == '-' || char == '.' {
			cleanedFamily.WriteRune(char)
		}
	}

	result := strings.TrimSpace(cleanedFamily.String())

	// Replace spaces with underscores for consistency
	result = strings.ReplaceAll(result, " ", "_")

	// Limit length to match database constraint
	if len(result) > 100 {
		result = result[:100]
	}

	// If result is empty after cleaning, return "unknown"
	if result == "" {
		return "unknown"
	}

	return result
}

// TransformDocument transforms a MongoDB document to a PostgreSQL record
func (p *PostgreSQLClient) TransformDocument(doc ThreatDocument) (*ThreatRecord, error) {
	// Generate UUID from MongoDB ObjectID
	id := uuid.New()

	// Debug: Log the raw document data BEFORE sanitization
	// log.Printf("DEBUG: Raw document %s - DestAddr: '%s', DestCountry: '%s'",
	// 	doc.ID.Hex(), doc.OptionalInformation.DestinationAddress, doc.OptionalInformation.DestinationCountry)

	// Sanitize all string fields first to prevent UTF-8 encoding issues
	doc.ASN = sanitizeUTF8String(doc.ASN)
	doc.ASNInfo = sanitizeUTF8String(doc.ASNInfo)
	doc.Category = sanitizeUTF8String(doc.Category)
	doc.SourceAddress = sanitizeUTF8String(doc.SourceAddress)
	doc.SourceCountry = sanitizeUTF8String(doc.SourceCountry)
	doc.OptionalInformation.DestinationAddress = sanitizeUTF8String(doc.OptionalInformation.DestinationAddress)
	doc.OptionalInformation.DestinationCountry = sanitizeUTF8String(doc.OptionalInformation.DestinationCountry)
	doc.OptionalInformation.DestinationPort = sanitizeUTF8String(doc.OptionalInformation.DestinationPort)
	doc.OptionalInformation.SourcePort = sanitizeUTF8String(doc.OptionalInformation.SourcePort)
	doc.OptionalInformation.Protocol = sanitizeUTF8String(doc.OptionalInformation.Protocol)
	doc.OptionalInformation.Family = sanitizeUTF8String(doc.OptionalInformation.Family)

	// Debug: Check for null bytes after sanitization
	if strings.Contains(doc.Category, "\x00") {
		log.Printf("WARNING: Category still contains null bytes after sanitization: %q", doc.Category)
		doc.Category = strings.ReplaceAll(doc.Category, "\x00", "")
	}

	// Validate and parse source address (required field)
	sourceIP, err := p.parseAndValidateIPAddress(doc.SourceAddress)
	if err != nil {
		return nil, fmt.Errorf("invalid source IP address '%s': %w", doc.SourceAddress, err)
	}

	// Normalize and get ASN ID (required field)
	normalizedASN := p.normalizeASN(doc.ASN)
	normalizedASNInfo := p.normalizeASNInfo(doc.ASNInfo)
	asnID, err := p.GetOrCreateAsnID(normalizedASN, normalizedASNInfo)
	if err != nil {
		return nil, fmt.Errorf("failed to get ASN ID for '%s': %w", normalizedASN, err)
	}

	// Normalize category (required field)
	normalizedCategory := p.normalizeCategory(doc.Category)
	if normalizedCategory == "" {
		return nil, fmt.Errorf("category cannot be empty after normalization")
	}

	// Set default timestamps if not present in MongoDB document
	createdAt := doc.CreatedAt
	if createdAt.IsZero() {
		createdAt = time.Now()
	}

	updatedAt := doc.UpdatedAt
	if updatedAt.IsZero() {
		updatedAt = time.Now()
	}

	record := &ThreatRecord{
		ID:            id,
		Timestamp:     doc.Timestamp,
		AsnID:         asnID,
		SourceAddress: sourceIP,
		Category:      normalizedCategory,
		CreatedAt:     createdAt,
		UpdatedAt:     updatedAt,
	}

	// Handle optional source country
	if doc.SourceCountry != "" {
		normalizedCountryCode := p.normalizeCountryCode(doc.SourceCountry)
		if normalizedCountryCode != "" {
			countryID, err := p.GetOrCreateCountryID(normalizedCountryCode, doc.SourceCountry)
			if err != nil {
				return nil, fmt.Errorf("failed to get source country ID for '%s': %w", normalizedCountryCode, err)
			}
			record.SourceCountryID = &countryID
		}
	}

	// Handle optional destination address with validation
	if doc.OptionalInformation.DestinationAddress != "" {
		// log.Printf("DEBUG: Processing destination address: '%s' for document %s",
		// doc.OptionalInformation.DestinationAddress, doc.ID.Hex())
		destIP, err := p.parseAndValidateIPAddress(doc.OptionalInformation.DestinationAddress)
		if err != nil {
			// Log warning but don't fail the entire record
			log.Printf("Warning: invalid destination IP address '%s' for document %s: %v",
				doc.OptionalInformation.DestinationAddress, doc.ID.Hex(), err)
		} else {
			record.DestinationAddress = &destIP
			// log.Printf("DEBUG: Successfully set destination address: %s", destIP.String())
		}
	} else {
		// log.Printf("DEBUG: No destination address found for document %s", doc.ID.Hex())
	}

	// Handle optional destination country
	if doc.OptionalInformation.DestinationCountry != "" {
		// log.Printf("DEBUG: Processing destination country: '%s' for document %s",
		// 	doc.OptionalInformation.DestinationCountry, doc.ID.Hex())
		normalizedCountryCode := p.normalizeCountryCode(doc.OptionalInformation.DestinationCountry)
		if normalizedCountryCode != "" {
			countryID, err := p.GetOrCreateCountryID(normalizedCountryCode, doc.OptionalInformation.DestinationCountry)
			if err != nil {
				return nil, fmt.Errorf("failed to get destination country ID for '%s': %w", normalizedCountryCode, err)
			}
			record.DestinationCountryID = &countryID
			// log.Printf("DEBUG: Successfully set destination country ID: %d", countryID)
		} else {
			// log.Printf("DEBUG: Country normalization returned empty for: '%s'", doc.OptionalInformation.DestinationCountry)
		}
	} else {
		// log.Printf("DEBUG: No destination country found for document %s", doc.ID.Hex())
	}

	// Handle optional source port with validation
	if doc.OptionalInformation.SourcePort != "" {
		port, err := p.parseAndValidatePort(doc.OptionalInformation.SourcePort)
		if err != nil {
			log.Printf("Warning: invalid source port '%s' for document %s: %v",
				doc.OptionalInformation.SourcePort, doc.ID.Hex(), err)
		} else {
			record.SourcePort = &port
		}
	}

	// Handle optional destination port with validation
	if doc.OptionalInformation.DestinationPort != "" {
		port, err := p.parseAndValidatePort(doc.OptionalInformation.DestinationPort)
		if err != nil {
			log.Printf("Warning: invalid destination port '%s' for document %s: %v",
				doc.OptionalInformation.DestinationPort, doc.ID.Hex(), err)
		} else {
			record.DestinationPort = &port
		}
	}

	// Handle optional protocol with normalization
	if doc.OptionalInformation.Protocol != "" {
		normalizedProtocol := p.normalizeProtocol(doc.OptionalInformation.Protocol)
		if normalizedProtocol != "" {
			protocolID, err := p.GetOrCreateProtocolID(normalizedProtocol)
			if err != nil {
				return nil, fmt.Errorf("failed to get protocol ID for '%s': %w", normalizedProtocol, err)
			}
			record.ProtocolID = &protocolID
		}
	}

	// Handle optional malware family with normalization
	if doc.OptionalInformation.Family != "" {
		normalizedFamily := p.normalizeMalwareFamily(doc.OptionalInformation.Family)
		if normalizedFamily != "" {
			familyID, err := p.GetOrCreateMalwareFamilyID(normalizedFamily)
			if err != nil {
				return nil, fmt.Errorf("failed to get malware family ID for '%s': %w", normalizedFamily, err)
			}
			record.MalwareFamilyID = &familyID
		}
	}

	return record, nil
}

// sanitizeUTF8String removes null bytes and other problematic characters that cause PostgreSQL UTF-8 errors
func sanitizeUTF8String(s string) string {
	if s == "" {
		return s
	}

	// First pass: remove null bytes using string replacement (most aggressive)
	s = strings.ReplaceAll(s, "\x00", "")

	// Second pass: remove other problematic control characters
	var cleaned strings.Builder
	for _, r := range s {
		// Skip problematic control characters but keep tab, newline, carriage return
		if r == 0 || (r < 32 && r != 9 && r != 10 && r != 13) {
			continue
		}
		cleaned.WriteRune(r)
	}

	result := cleaned.String()

	// Third pass: ensure the result is valid UTF-8
	if !utf8.ValidString(result) {
		// If still not valid UTF-8, convert invalid sequences to replacement character
		result = strings.ToValidUTF8(result, "")
	}

	// Final pass: remove any remaining null bytes that might have been introduced
	result = strings.ReplaceAll(result, "\x00", "")

	return result
}
