package main

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"strings"
)

// MITRE ATT&CK Data Structures
type MitreAttackFramework struct {
	Objects []MitreObject `json:"objects"`
}

type MitreObject struct {
	Type                 string           `json:"type"`
	ID                   string           `json:"id"`
	CreatedByRef         string           `json:"created_by_ref"`
	Created              string           `json:"created"`
	Modified             string           `json:"modified"`
	Name                 string           `json:"name"`
	Description          string           `json:"description"`
	KillChainPhases      []KillChainPhase `json:"kill_chain_phases,omitempty"`
	ExternalRefs         []ExternalRef    `json:"external_references,omitempty"`
	XMitreIsSubtechnique bool             `json:"x_mitre_is_subtechnique,omitempty"`
	XMitrePlatforms      []string         `json:"x_mitre_platforms,omitempty"`
}

type KillChainPhase struct {
	KillChainName string `json:"kill_chain_name"`
	PhaseName     string `json:"phase_name"`
}

type ExternalRef struct {
	SourceName  string `json:"source_name"`
	ExternalID  string `json:"external_id"`
	URL         string `json:"url"`
	Description string `json:"description"`
}

// TechniqueMapper maps threat categories to MITRE ATT&CK techniques
type TechniqueMapper struct {
	techniques map[string][]string
}

// NewTechniqueMapper initializes technique mapping
func NewTechniqueMapper() *TechniqueMapper {
	mapper := &TechniqueMapper{
		techniques: make(map[string][]string),
	}

	// Initialize common mappings based on your threat categories
	mapper.techniques["bot"] = []string{"T1071", "T1090", "T1105"}     // Application Layer Protocol, Proxy, Ingress Tool Transfer
	mapper.techniques["c2"] = []string{"T1071", "T1573", "T1008"}      // Application Layer Protocol, Encrypted Channel, Fallback Channels
	mapper.techniques["malware"] = []string{"T1203", "T1204", "T1566"} // Exploitation for Client Execution, User Execution, Phishing
	mapper.techniques["scan"] = []string{"T1046", "T1018", "T1135"}    // Network Service Scanning, Remote System Discovery, Network Share Discovery
	mapper.techniques["brute"] = []string{"T1110", "T1021", "T1078"}   // Brute Force, Remote Services, Valid Accounts
	mapper.techniques["exploit"] = []string{"T1190", "T1210", "T1068"} // Exploit Public-Facing Application, Exploitation of Remote Services, Exploitation for Privilege Escalation

	return mapper
}

// DownloadMitreAttackFramework downloads the latest MITRE ATT&CK framework
func DownloadMitreAttackFramework() (*MitreAttackFramework, error) {
	// MITRE ATT&CK Enterprise framework (free download)
	url := "https://raw.githubusercontent.com/mitre/cti/master/enterprise-attack/enterprise-attack.json"

	resp, err := http.Get(url)
	if err != nil {
		return nil, fmt.Errorf("failed to download MITRE ATT&CK framework: %v", err)
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("failed to read response body: %v", err)
	}

	var framework MitreAttackFramework
	err = json.Unmarshal(body, &framework)
	if err != nil {
		return nil, fmt.Errorf("failed to parse MITRE ATT&CK framework: %v", err)
	}

	return &framework, nil
}

// MapCategoryToTechniques maps threat event categories to MITRE techniques
func (tm *TechniqueMapper) MapCategoryToTechniques(category string) []string {
	category = strings.ToLower(strings.TrimSpace(category))

	// Direct mapping
	if techniques, exists := tm.techniques[category]; exists {
		return techniques
	}

	// Fuzzy matching for partial category names
	for key, techniques := range tm.techniques {
		if strings.Contains(category, key) {
			return techniques
		}
	}

	return []string{} // No mapping found
}

// EnrichThreatEventsWithMitre enriches threat events with MITRE ATT&CK techniques
func EnrichThreatEventsWithMitre() {
	// Download latest MITRE framework
	framework, err := DownloadMitreAttackFramework()
	if err != nil {
		fmt.Printf("Error downloading MITRE framework: %v\n", err)
		return
	}

	fmt.Printf("Downloaded MITRE ATT&CK framework with %d objects\n", len(framework.Objects))

	// Create technique lookup
	techniqueDetails := make(map[string]MitreObject)
	for _, obj := range framework.Objects {
		if obj.Type == "attack-pattern" {
			for _, ref := range obj.ExternalRefs {
				if ref.SourceName == "mitre-attack" && strings.HasPrefix(ref.ExternalID, "T") {
					techniqueDetails[ref.ExternalID] = obj
				}
			}
		}
	}

	// Initialize mapper
	mapper := NewTechniqueMapper()

	// Example: Map your threat categories
	categories := []string{"bot", "c2", "malware", "scan", "brute", "exploit"}

	for _, category := range categories {
		techniques := mapper.MapCategoryToTechniques(category)
		fmt.Printf("\nCategory: %s\n", category)

		for _, techID := range techniques {
			if detail, exists := techniqueDetails[techID]; exists {
				fmt.Printf("  - %s: %s\n", techID, detail.Name)
				fmt.Printf("    Description: %s\n", strings.Split(detail.Description, ".")[0])

				// Here you would update your database with the mapping
				updateSQL := `
					INSERT INTO "ThreatEventTechniques" ("ThreatEventId", "TechniqueId", "DetectionConfidence")
					SELECT te."Id", at."Id", 0.8
					FROM "ThreatEvents" te
					CROSS JOIN "AttackTechniques" at
					WHERE te."Category" = $1 AND at."MitreId" = $2
					ON CONFLICT DO NOTHING`

				// Execute: db.Exec(updateSQL, category, techID)
			}
		}
	}
}

// CreateMitreAttackTables creates the necessary database tables
func CreateMitreAttackTables() string {
	return `
-- Create MITRE ATT&CK technique storage
CREATE TABLE IF NOT EXISTS "AttackTechniques" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "MitreId" VARCHAR(20) UNIQUE NOT NULL,
    "TechniqueName" VARCHAR(200) NOT NULL,
    "TacticCategory" VARCHAR(100),
    "SubTechnique" VARCHAR(200),
    "Platform" VARCHAR(100),
    "Description" TEXT,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);

-- Create technique-event mapping
CREATE TABLE IF NOT EXISTS "ThreatEventTechniques" (
    "ThreatEventId" UUID,
    "TechniqueId" UUID,
    "DetectionConfidence" DECIMAL(3,2) DEFAULT 0.5,
    "DetectedAt" TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY ("ThreatEventId", "TechniqueId"),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("TechniqueId") REFERENCES "AttackTechniques"("Id")
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS "idx_attack_techniques_mitre_id" ON "AttackTechniques"("MitreId");
CREATE INDEX IF NOT EXISTS "idx_threat_event_techniques_event" ON "ThreatEventTechniques"("ThreatEventId");
CREATE INDEX IF NOT EXISTS "idx_threat_event_techniques_technique" ON "ThreatEventTechniques"("TechniqueId");

-- Insert common MITRE ATT&CK techniques
INSERT INTO "AttackTechniques" ("MitreId", "TechniqueName", "TacticCategory", "Description") VALUES
('T1071', 'Application Layer Protocol', 'Command and Control', 'Adversaries may communicate using OSI application layer protocols'),
('T1090', 'Proxy', 'Command and Control', 'Adversaries may use a connection proxy to direct network traffic'),
('T1105', 'Ingress Tool Transfer', 'Command and Control', 'Adversaries may transfer tools or other files'),
('T1573', 'Encrypted Channel', 'Command and Control', 'Adversaries may employ a known encryption algorithm'),
('T1008', 'Fallback Channels', 'Command and Control', 'Adversaries may use fallback or alternate communication channels'),
('T1203', 'Exploitation for Client Execution', 'Execution', 'Adversaries may exploit software vulnerabilities in client applications'),
('T1204', 'User Execution', 'Execution', 'An adversary may rely upon specific actions by a user'),
('T1566', 'Phishing', 'Initial Access', 'Adversaries may send phishing messages to gain access'),
('T1046', 'Network Service Scanning', 'Discovery', 'Adversaries may attempt to get a listing of services'),
('T1018', 'Remote System Discovery', 'Discovery', 'Adversaries may attempt to get a listing of other systems'),
('T1135', 'Network Share Discovery', 'Discovery', 'Adversaries may look for folders and drives shared on remote systems'),
('T1110', 'Brute Force', 'Credential Access', 'Adversaries may use brute force techniques to gain access'),
('T1021', 'Remote Services', 'Lateral Movement', 'Adversaries may use valid accounts to log into a service'),
('T1078', 'Valid Accounts', 'Defense Evasion', 'Adversaries may obtain and abuse credentials of existing accounts'),
('T1190', 'Exploit Public-Facing Application', 'Initial Access', 'Adversaries may attempt to take advantage of a weakness'),
('T1210', 'Exploitation of Remote Services', 'Lateral Movement', 'Adversaries may exploit remote services to gain unauthorized access'),
('T1068', 'Exploitation for Privilege Escalation', 'Privilege Escalation', 'Adversaries may exploit software vulnerabilities')
ON CONFLICT ("MitreId") DO NOTHING;
`
}
