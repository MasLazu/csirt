package main

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"net"
	"sort"
	"strings"
	"sync"
	"time"
	"unicode/utf8"

	"github.com/google/uuid"
	"github.com/lib/pq"
	_ "github.com/lib/pq"
)

// PostgreSQLClient wraps PostgreSQL operations
type PostgreSQLClient struct {
	db     *sql.DB
	config PostgreSQLConfig

	// Removed per-table prepared insert statements except for ThreatEvents
	insertThreatStmt *sql.Stmt

	asnCache           map[string]uuid.UUID
	countryCache       map[string]uuid.UUID
	protocolCache      map[string]uuid.UUID
	malwareFamilyCache map[string]uuid.UUID

	useCopy       bool
	copyThreshold int
	adaptiveCopy  bool
	copyMin       int
	copyMax       int

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
	AsnRegistryID        uuid.UUID
	SourceAddress        net.IP
	SourceCountryID      *uuid.UUID
	DestinationAddress   *net.IP
	DestinationCountryID *uuid.UUID
	SourcePort           *int
	DestinationPort      *int
	ProtocolID           *uuid.UUID
	Category             string
	MalwareFamilyID      *uuid.UUID
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
		asnCache:           make(map[string]uuid.UUID),
		countryCache:       make(map[string]uuid.UUID),
		protocolCache:      make(map[string]uuid.UUID),
		malwareFamilyCache: make(map[string]uuid.UUID),
		useCopy:            migrationConfig.UseCopy,
		copyThreshold:      migrationConfig.CopyThreshold,
		adaptiveCopy:       migrationConfig.AdaptiveCopy,
		copyMin:            migrationConfig.CopyMinThreshold,
		copyMax:            migrationConfig.CopyMaxThreshold,
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
	if p.insertThreatStmt != nil {
		_ = p.insertThreatStmt.Close()
	}
	return p.db.Close()
}

// prepareStatements prepares all SQL statements for batch operations
func (p *PostgreSQLClient) prepareStatements() error {
	var err error

	p.insertThreatStmt, err = p.db.Prepare(`
		INSERT INTO "ThreatEvents" (
			"Id","Timestamp","AsnRegistryId","SourceAddress","SourceCountryId",
			"DestinationAddress","DestinationCountryId","SourcePort","DestinationPort",
			"ProtocolId","Category","MalwareFamilyId","CreatedAt","UpdatedAt"
		) VALUES ($1,$2,$3,$4,$5,$6,$7,$8,$9,$10,$11,$12,$13,$14)`)
	if err != nil {
		return fmt.Errorf("failed to prepare ThreatEvents insert: %w", err)
	}
	return nil
}

// loadLookupCaches loads existing lookup data into memory caches
func (p *PostgreSQLClient) loadLookupCaches() error {
	log.Println("Loading ASN cache (AsnRegistries)...")
	ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel()

	rows, err := p.db.QueryContext(ctx, `SELECT "Id", "Number" FROM "AsnRegistries" LIMIT 200000`)
	if err != nil {
		log.Printf("Warning: Could not load ASN cache (table may not exist): %v", err)
	} else {
		defer rows.Close()
		count := 0
		for rows.Next() {
			var id uuid.UUID
			var number string
			if err := rows.Scan(&id, &number); err != nil {
				log.Printf("Warning: failed to scan AsnRegistries row: %v", err)
				continue
			}
			if number != "" {
				p.asnCache[number] = id
				count++
			}
		}
		log.Printf("Loaded %d ASN entries", count)
	}

	log.Println("Loading country cache (Countries)...")
	ctx2, cancel2 := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel2()
	rows, err = p.db.QueryContext(ctx2, `SELECT "Id", "Code" FROM "Countries" LIMIT 10000`)
	if err != nil {
		log.Printf("Warning: Could not load Countries cache: %v", err)
	} else {
		defer rows.Close()
		count := 0
		for rows.Next() {
			var id uuid.UUID
			var code string
			if err := rows.Scan(&id, &code); err != nil {
				log.Printf("Warning: failed to scan Countries row: %v", err)
				continue
			}
			if code != "" {
				p.countryCache[code] = id
				count++
			}
		}
		log.Printf("Loaded %d country entries", count)
	}

	log.Println("Loading protocol cache (Protocols)...")
	ctx3, cancel3 := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel3()
	rows, err = p.db.QueryContext(ctx3, `SELECT "Id", "Name" FROM "Protocols" LIMIT 10000`)
	if err != nil {
		log.Printf("Warning: Could not load Protocols cache: %v", err)
	} else {
		defer rows.Close()
		count := 0
		for rows.Next() {
			var id uuid.UUID
			var name string
			if err := rows.Scan(&id, &name); err != nil {
				log.Printf("Warning: failed to scan Protocols row: %v", err)
				continue
			}
			if name != "" {
				p.protocolCache[name] = id
				count++
			}
		}
		log.Printf("Loaded %d protocol entries", count)
	}

	log.Println("Loading malware family cache (MalwareFamilies)...")
	ctx4, cancel4 := context.WithTimeout(context.Background(), 30*time.Second)
	defer cancel4()
	rows, err = p.db.QueryContext(ctx4, `SELECT "Id", "Name" FROM "MalwareFamilies" LIMIT 10000`)
	if err != nil {
		log.Printf("Warning: Could not load MalwareFamilies cache: %v", err)
	} else {
		defer rows.Close()
		count := 0
		for rows.Next() {
			var id uuid.UUID
			var name string
			if err := rows.Scan(&id, &name); err != nil {
				log.Printf("Warning: failed to scan MalwareFamilies row: %v", err)
				continue
			}
			if name != "" {
				p.malwareFamilyCache[name] = id
				count++
			}
		}
		log.Printf("Loaded %d malware family entries", count)
	}

	log.Printf("Loaded lookup caches: %d ASNs, %d countries, %d protocols, %d malware families",
		len(p.asnCache), len(p.countryCache), len(p.protocolCache), len(p.malwareFamilyCache))

	return nil
}

// GetOrCreateAsnID gets or creates an ASN record and returns its ID
func (p *PostgreSQLClient) GetOrCreateAsnID(asn, description string) (uuid.UUID, error) {
	// Cache check
	p.cacheMutex.RLock()
	if id, ok := p.asnCache[asn]; ok {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	id, err := p.getOrCreateUUID("AsnRegistries", "Number", asn, map[string]string{"Description": description})
	if err != nil {
		return uuid.Nil, err
	}
	p.cacheMutex.Lock()
	p.asnCache[asn] = id
	p.cacheMutex.Unlock()
	return id, nil
}

// GetOrCreateCountryID gets or creates a country record and returns its ID
func (p *PostgreSQLClient) GetOrCreateCountryID(code, name string) (uuid.UUID, error) {
	p.cacheMutex.RLock()
	if id, ok := p.countryCache[code]; ok {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	id, err := p.getOrCreateUUID("Countries", "Code", code, map[string]string{"Name": name})
	if err != nil {
		return uuid.Nil, err
	}
	p.cacheMutex.Lock()
	p.countryCache[code] = id
	p.cacheMutex.Unlock()
	return id, nil
}

// GetOrCreateProtocolID gets or creates a protocol record and returns its ID
func (p *PostgreSQLClient) GetOrCreateProtocolID(name string) (uuid.UUID, error) {
	p.cacheMutex.RLock()
	if id, ok := p.protocolCache[name]; ok {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	id, err := p.getOrCreateUUID("Protocols", "Name", name, nil)
	if err != nil {
		return uuid.Nil, err
	}
	p.cacheMutex.Lock()
	p.protocolCache[name] = id
	p.cacheMutex.Unlock()
	return id, nil
}

// GetOrCreateMalwareFamilyID gets or creates a malware family record and returns its ID
func (p *PostgreSQLClient) GetOrCreateMalwareFamilyID(name string) (uuid.UUID, error) {
	p.cacheMutex.RLock()
	if id, ok := p.malwareFamilyCache[name]; ok {
		p.cacheMutex.RUnlock()
		return id, nil
	}
	p.cacheMutex.RUnlock()

	id, err := p.getOrCreateUUID("MalwareFamilies", "Name", name, nil)
	if err != nil {
		return uuid.Nil, err
	}
	p.cacheMutex.Lock()
	p.malwareFamilyCache[name] = id
	p.cacheMutex.Unlock()
	return id, nil
}

// InsertThreatBatch inserts a batch of threat records
func (p *PostgreSQLClient) InsertThreatBatch(threats []ThreatRecord) error {
	if len(threats) == 0 {
		return nil
	}

	threshold := p.copyThreshold
	if p.adaptiveCopy {
		// Placeholder adaptive logic: grow threshold modestly if very large batches observed
		if len(threats) > threshold*4 && threshold < p.copyMax {
			threshold = min(p.copyMax, threshold*2)
		} else if len(threats) < threshold/2 && threshold > p.copyMin {
			threshold = max(p.copyMin, threshold/2)
		}
		p.copyThreshold = threshold
	}

	if p.useCopy && len(threats) >= threshold {
		if err := p.copyThreatBatch(threats); err == nil {
			return nil
		} else {
			log.Printf("COPY failed, fallback to row inserts: %v", err)
		}
	}

	// Fallback per-row prepared statement inside transaction
	tx, err := p.db.Begin()
	if err != nil {
		return fmt.Errorf("failed to begin transaction: %w", err)
	}
	defer tx.Rollback()

	stmt := tx.Stmt(p.insertThreatStmt)
	defer stmt.Close()

	for _, threat := range threats {
		var srcCountry, dstCountry, proto, mal *uuid.UUID
		if threat.SourceCountryID != nil {
			srcCountry = threat.SourceCountryID
		}
		if threat.DestinationCountryID != nil {
			dstCountry = threat.DestinationCountryID
		}
		if threat.ProtocolID != nil {
			proto = threat.ProtocolID
		}
		if threat.MalwareFamilyID != nil {
			mal = threat.MalwareFamilyID
		}
		var destAddrStr *string
		if threat.DestinationAddress != nil {
			s := threat.DestinationAddress.String()
			destAddrStr = &s
		}
		if _, err := stmt.Exec(
			threat.ID,
			threat.Timestamp,
			threat.AsnRegistryID,
			threat.SourceAddress.String(),
			srcCountry,
			destAddrStr,
			dstCountry,
			threat.SourcePort,
			threat.DestinationPort,
			proto,
			threat.Category,
			mal,
			threat.CreatedAt,
			threat.UpdatedAt,
		); err != nil {
			return fmt.Errorf("insert threat %s: %w", threat.ID, err)
		}
	}
	if err := tx.Commit(); err != nil {
		return fmt.Errorf("failed to commit transaction: %w", err)
	}
	return nil
}

// copyThreatBatch performs bulk ingestion using PostgreSQL COPY protocol
func (p *PostgreSQLClient) copyThreatBatch(threats []ThreatRecord) error {
	conn, err := p.db.Conn(context.Background())
	if err != nil {
		return err
	}
	defer conn.Close()

	// Columns list mirrors prepared insert ordering
	copyInStmt := pq.CopyIn("ThreatEvents",
		"Id", "Timestamp", "AsnRegistryId", "SourceAddress", "SourceCountryId",
		"DestinationAddress", "DestinationCountryId", "SourcePort", "DestinationPort",
		"ProtocolId", "Category", "MalwareFamilyId", "CreatedAt", "UpdatedAt",
	)

	// Use explicit transaction for COPY for atomicity
	var tx *sql.Tx
	if err := conn.Raw(func(driverConn any) error { return nil }); err != nil {
		return err
	}
	tx, err = conn.BeginTx(context.Background(), nil)
	if err != nil {
		return err
	}
	stmt, err := tx.Prepare(copyInStmt)
	if err != nil {
		tx.Rollback()
		return fmt.Errorf("prepare COPY failed: %w", err)
	}

	for _, t := range threats {
		var destAddr *string
		if t.DestinationAddress != nil {
			s := t.DestinationAddress.String()
			destAddr = &s
		}
		var srcCountry, dstCountry, proto, mal interface{}
		if t.SourceCountryID != nil {
			srcCountry = *t.SourceCountryID
		} else {
			srcCountry = nil
		}
		if t.DestinationCountryID != nil {
			dstCountry = *t.DestinationCountryID
		} else {
			dstCountry = nil
		}
		if t.ProtocolID != nil {
			proto = *t.ProtocolID
		} else {
			proto = nil
		}
		if t.MalwareFamilyID != nil {
			mal = *t.MalwareFamilyID
		} else {
			mal = nil
		}
		if _, err := stmt.Exec(
			&t.ID,
			&t.Timestamp,
			&t.AsnRegistryID,
			t.SourceAddress.String(),
			srcCountry,
			destAddr,
			dstCountry,
			t.SourcePort,
			t.DestinationPort,
			proto,
			&t.Category,
			mal,
			&t.CreatedAt,
			&t.UpdatedAt,
		); err != nil {
			stmt.Close()
			tx.Rollback()
			return fmt.Errorf("COPY exec failed: %w", err)
		}
	}
	if _, err := stmt.Exec(); err != nil {
		stmt.Close()
		tx.Rollback()
		return fmt.Errorf("COPY finalize failed: %w", err)
	}
	if err := stmt.Close(); err != nil {
		tx.Rollback()
		return fmt.Errorf("COPY close failed: %w", err)
	}
	if err := tx.Commit(); err != nil {
		return fmt.Errorf("COPY commit failed: %w", err)
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
	asnUUID, err := p.GetOrCreateAsnID(normalizedASN, normalizedASNInfo)
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
		AsnRegistryID: asnUUID,
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

// getOrCreateUUID is a generic helper used for dynamic lookup table population (Approach A)
func (p *PostgreSQLClient) getOrCreateUUID(table, keyColumn, value string, extra map[string]string) (uuid.UUID, error) {
	value = strings.TrimSpace(value)
	if value == "" {
		return uuid.Nil, fmt.Errorf("%s value cannot be empty", keyColumn)
	}

	// Try select first
	selectSQL := fmt.Sprintf(`SELECT "Id" FROM "%s" WHERE "%s" = $1`, table, keyColumn)
	var existing uuid.UUID
	err := p.db.QueryRow(selectSQL, value).Scan(&existing)
	if err == nil {
		return existing, nil
	}
	if err != nil && err != sql.ErrNoRows { // retain check
		return uuid.Nil, fmt.Errorf("lookup %s.%s failed: %w", table, keyColumn, err)
	}

	// Build insert (ensure CreatedAt present due to NOT NULL constraint in EF model)
	newID := uuid.New()
	columns := []string{`"Id"`, fmt.Sprintf(`"%s"`, keyColumn), `"CreatedAt"`}
	createdAt := time.Now().UTC()
	args := []any{newID, value, createdAt}

	if extra == nil {
		extra = map[string]string{}
	}
	// Prevent caller from overriding CreatedAt accidentally
	delete(extra, "CreatedAt")

	if len(extra) > 0 {
		keys := make([]string, 0, len(extra))
		for k := range extra {
			keys = append(keys, k)
		}
		sort.Strings(keys)
		for _, k := range keys {
			columns = append(columns, fmt.Sprintf(`"%s"`, k))
			args = append(args, extra[k])
		}
	}

	placeholders := make([]string, len(columns))
	for i := range placeholders {
		placeholders[i] = fmt.Sprintf("$%d", i+1)
	}

	insertSQL := fmt.Sprintf(`INSERT INTO "%s" (%s) VALUES (%s) ON CONFLICT ("%s") DO NOTHING RETURNING "Id"`,
		table,
		strings.Join(columns, ","),
		strings.Join(placeholders, ","),
		keyColumn,
	)

	// Try insert (with RETURNING). If DO NOTHING triggered, we must re-select.
	err = p.db.QueryRow(insertSQL, args...).Scan(&existing)
	if err == sql.ErrNoRows {
		// Conflict path, reselect
		if e2 := p.db.QueryRow(selectSQL, value).Scan(&existing); e2 != nil {
			return uuid.Nil, fmt.Errorf("post-conflict select %s.%s failed: %w", table, keyColumn, e2)
		}
		return existing, nil
	}
	if err != nil {
		return uuid.Nil, fmt.Errorf("insert %s.%s failed: %w", table, keyColumn, err)
	}
	return existing, nil
}

func min(a, b int) int {
	if a < b {
		return a
	}
	return b
}
func max(a, b int) int {
	if a > b {
		return a
	}
	return b
}
