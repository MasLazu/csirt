-- Complete Database Schema for Enriched Threat Intelligence Platform
-- This script adds all enrichment capabilities to your existing ThreatEvents table

-- 1. Add enrichment columns to existing ThreatEvents table
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SourceLatitude" DECIMAL(10,8);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SourceLongitude" DECIMAL(11,8);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SourceCity" VARCHAR(100);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SourceRegion" VARCHAR(100);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SourceISP" VARCHAR(200);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SourceOrganization" VARCHAR(200);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "DestinationLatitude" DECIMAL(10,8);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "DestinationLongitude" DECIMAL(11,8);

-- Risk and intelligence scoring
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "RiskScore" DECIMAL(5,2);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "ConfidenceLevel" DECIMAL(3,2);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SeverityLevel" VARCHAR(20);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "ThreatTags" TEXT[];

-- Temporal behavior patterns
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "DayOfWeek" INT;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "HourOfDay" INT;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "IsWeekend" BOOLEAN;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "IsHoliday" BOOLEAN;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "TimeZoneOffset" INT;

-- Network behavior analysis
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "SessionId" UUID;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "BytesTransferred" BIGINT;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "PacketCount" INT;
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "Duration" INT; -- seconds
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "ConnectionState" VARCHAR(20);

-- Machine learning and behavioral analysis
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "AnomalyScore" DECIMAL(5,2);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "ClusterGroup" VARCHAR(50);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "PredictedThreatType" VARCHAR(100);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "MachineLearningModelVersion" VARCHAR(20);
ALTER TABLE "ThreatEvents" ADD COLUMN IF NOT EXISTS "BehaviorProfile" JSONB;

-- 2. Create Threat Campaigns table for attack sequence tracking
CREATE TABLE IF NOT EXISTS "ThreatCampaigns" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "CampaignName" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "FirstSeen" TIMESTAMP NOT NULL,
    "LastSeen" TIMESTAMP NOT NULL,
    "Status" VARCHAR(20) DEFAULT 'active', -- 'active', 'dormant', 'ended'
    "Confidence" DECIMAL(3,2) DEFAULT 0.5, -- 0.00 to 1.00
    "ThreatLevel" VARCHAR(20) DEFAULT 'medium', -- 'low', 'medium', 'high', 'critical'
    "AttackVectors" TEXT[],
    "AttributedActors" TEXT[],
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ThreatEventCampaigns" (
    "ThreatEventId" UUID,
    "CampaignId" UUID,
    "SequenceNumber" INT,
    "Role" VARCHAR(50), -- 'initiator', 'follow-up', 'persistence', 'exfiltration'
    "AssignedAt" TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY ("ThreatEventId", "CampaignId"),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("CampaignId") REFERENCES "ThreatCampaigns"("Id")
);

-- 3. Create Threat Intelligence Feeds integration
CREATE TABLE IF NOT EXISTS "ThreatIntelFeeds" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "SourceName" VARCHAR(100) NOT NULL,
    "FeedType" VARCHAR(50) NOT NULL, -- 'commercial', 'opensource', 'government'
    "ApiEndpoint" TEXT,
    "LastUpdated" TIMESTAMP,
    "Reliability" DECIMAL(3,2) DEFAULT 0.5,
    "IsActive" BOOLEAN DEFAULT true,
    "RateLimit" INT, -- requests per hour
    "ApiKey" TEXT, -- encrypted
    "CreatedAt" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ThreatEventIntelligence" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ThreatEventId" UUID NOT NULL,
    "IntelFeedId" UUID NOT NULL,
    "IndicatorType" VARCHAR(50) NOT NULL, -- 'ip', 'domain', 'hash', 'signature'
    "IndicatorValue" TEXT NOT NULL,
    "ThreatType" VARCHAR(100),
    "MaliciousProbability" DECIMAL(3,2),
    "FirstSeen" TIMESTAMP,
    "LastSeen" TIMESTAMP,
    "ReportedBy" TEXT[],
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("IntelFeedId") REFERENCES "ThreatIntelFeeds"("Id")
);

-- 4. Create MITRE ATT&CK framework integration
CREATE TABLE IF NOT EXISTS "AttackTechniques" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "MitreId" VARCHAR(20) UNIQUE NOT NULL,
    "TechniqueName" VARCHAR(200) NOT NULL,
    "TacticCategory" VARCHAR(100),
    "SubTechnique" VARCHAR(200),
    "Platform" VARCHAR(100),
    "Description" TEXT,
    "KillChainPhase" VARCHAR(100),
    "DataSources" TEXT[],
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ThreatEventTechniques" (
    "ThreatEventId" UUID,
    "TechniqueId" UUID,
    "DetectionConfidence" DECIMAL(3,2) DEFAULT 0.5,
    "DetectionMethod" VARCHAR(100), -- 'signature', 'behavior', 'anomaly'
    "Evidence" TEXT,
    "DetectedAt" TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY ("ThreatEventId", "TechniqueId"),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("TechniqueId") REFERENCES "AttackTechniques"("Id")
);

-- 5. Create Asset and Target context
CREATE TABLE IF NOT EXISTS "Assets" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "AssetName" VARCHAR(100) NOT NULL,
    "AssetType" VARCHAR(50) NOT NULL, -- 'server', 'workstation', 'iot', 'network'
    "IPAddress" INET,
    "Hostname" VARCHAR(255),
    "Criticality" VARCHAR(20) DEFAULT 'medium', -- 'low', 'medium', 'high', 'critical'
    "BusinessUnit" VARCHAR(100),
    "Owner" VARCHAR(100),
    "Location" VARCHAR(100),
    "OperatingSystem" VARCHAR(100),
    "LastAssessment" TIMESTAMP,
    "SecurityScore" DECIMAL(5,2),
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ThreatEventAssets" (
    "ThreatEventId" UUID,
    "AssetId" UUID,
    "ImpactType" VARCHAR(50) NOT NULL, -- 'attempted', 'successful', 'blocked'
    "ImpactLevel" VARCHAR(20), -- 'none', 'low', 'medium', 'high', 'critical'
    "DetectedAt" TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY ("ThreatEventId", "AssetId"),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("AssetId") REFERENCES "Assets"("Id")
);

-- 6. Create Threat Actor attribution
CREATE TABLE IF NOT EXISTS "ThreatActors" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ActorName" VARCHAR(100) NOT NULL,
    "ActorType" VARCHAR(50) NOT NULL, -- 'apt', 'cybercriminal', 'hacktivist', 'insider'
    "FirstSeen" TIMESTAMP,
    "LastSeen" TIMESTAMP,
    "OriginCountry" VARCHAR(2),
    "Sophistication" VARCHAR(20) DEFAULT 'medium', -- 'low', 'medium', 'high', 'advanced'
    "Motivation" VARCHAR(100),
    "Capabilities" TEXT[],
    "KnownAliases" TEXT[],
    "Attribution" TEXT, -- Attribution details
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ThreatEventAttribution" (
    "ThreatEventId" UUID,
    "ActorId" UUID,
    "AttributionConfidence" DECIMAL(3,2) DEFAULT 0.5,
    "AttributionMethod" VARCHAR(100), -- 'signature', 'behavior', 'infrastructure'
    "Evidence" TEXT,
    "AttributedAt" TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY ("ThreatEventId", "ActorId"),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("ActorId") REFERENCES "ThreatActors"("Id")
);

-- 7. Create Incident Response integration
CREATE TABLE IF NOT EXISTS "Incidents" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "IncidentNumber" VARCHAR(50) UNIQUE NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Status" VARCHAR(20) DEFAULT 'open', -- 'open', 'investigating', 'contained', 'resolved'
    "Severity" VARCHAR(20) DEFAULT 'medium',
    "Priority" VARCHAR(20) DEFAULT 'medium',
    "AssignedTo" VARCHAR(100),
    "CreatedBy" VARCHAR(100),
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "ResolvedAt" TIMESTAMP,
    "EstimatedImpact" DECIMAL(15,2),
    "ResponseTimeMinutes" INT,
    "ContainmentTimeMinutes" INT,
    "RecoveryTimeMinutes" INT
);

CREATE TABLE IF NOT EXISTS "ThreatEventIncidents" (
    "ThreatEventId" UUID,
    "IncidentId" UUID,
    "Role" VARCHAR(50) NOT NULL, -- 'trigger', 'related', 'evidence'
    "AssignedAt" TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY ("ThreatEventId", "IncidentId"),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id"),
    FOREIGN KEY ("IncidentId") REFERENCES "Incidents"("Id")
);

-- 8. Create Machine Learning predictions and analysis
CREATE TABLE IF NOT EXISTS "ThreatPredictions" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "ThreatEventId" UUID,
    "PredictionType" VARCHAR(50) NOT NULL, -- 'next_target', 'escalation', 'campaign_evolution'
    "PredictedValue" TEXT NOT NULL,
    "Confidence" DECIMAL(3,2) NOT NULL,
    "ModelUsed" VARCHAR(100) NOT NULL,
    "ModelVersion" VARCHAR(20),
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "ValidatedAt" TIMESTAMP,
    "IsAccurate" BOOLEAN,
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id")
);

-- 9. Create Protocol-specific metadata
CREATE TABLE IF NOT EXISTS "ThreatEventMetadata" (
    "ThreatEventId" UUID PRIMARY KEY,
    "HttpUserAgent" TEXT,
    "HttpMethod" VARCHAR(10),
    "HttpStatusCode" INT,
    "HttpUri" TEXT,
    "DnsQuery" VARCHAR(255),
    "DnsResponse" TEXT,
    "SslCertificateHash" VARCHAR(64),
    "SslSubject" TEXT,
    "PayloadSignature" VARCHAR(128),
    "PayloadSize" INT,
    "EmailSubject" TEXT,
    "EmailSender" VARCHAR(255),
    "FileHash" VARCHAR(64),
    "FileName" VARCHAR(255),
    "ProcessName" VARCHAR(255),
    "CommandLine" TEXT,
    "RegistryKey" TEXT,
    "NetworkProtocol" VARCHAR(20),
    FOREIGN KEY ("ThreatEventId") REFERENCES "ThreatEvents"("Id")
);

-- 10. Create indexes for performance optimization
CREATE INDEX IF NOT EXISTS "idx_threatevents_source_lat_lng" ON "ThreatEvents"("SourceLatitude", "SourceLongitude");
CREATE INDEX IF NOT EXISTS "idx_threatevents_risk_score" ON "ThreatEvents"("RiskScore" DESC);
CREATE INDEX IF NOT EXISTS "idx_threatevents_anomaly_score" ON "ThreatEvents"("AnomalyScore" DESC);
CREATE INDEX IF NOT EXISTS "idx_threatevents_cluster_group" ON "ThreatEvents"("ClusterGroup");
CREATE INDEX IF NOT EXISTS "idx_threatevents_session_id" ON "ThreatEvents"("SessionId");
CREATE INDEX IF NOT EXISTS "idx_threatevents_day_hour" ON "ThreatEvents"("DayOfWeek", "HourOfDay");
CREATE INDEX IF NOT EXISTS "idx_threatevents_source_isp" ON "ThreatEvents"("SourceISP");

-- Campaign indexes
CREATE INDEX IF NOT EXISTS "idx_threatcampaigns_status_level" ON "ThreatCampaigns"("Status", "ThreatLevel");
CREATE INDEX IF NOT EXISTS "idx_threatcampaigns_timerange" ON "ThreatCampaigns"("FirstSeen", "LastSeen");

-- Intelligence feed indexes
CREATE INDEX IF NOT EXISTS "idx_threateventintel_indicator" ON "ThreatEventIntelligence"("IndicatorType", "IndicatorValue");
CREATE INDEX IF NOT EXISTS "idx_threateventintel_probability" ON "ThreatEventIntelligence"("MaliciousProbability" DESC);

-- MITRE ATT&CK indexes
CREATE INDEX IF NOT EXISTS "idx_attacktechniques_mitre_tactic" ON "AttackTechniques"("MitreId", "TacticCategory");
CREATE INDEX IF NOT EXISTS "idx_threateventtechniques_confidence" ON "ThreatEventTechniques"("DetectionConfidence" DESC);

-- Incident response indexes
CREATE INDEX IF NOT EXISTS "idx_incidents_status_severity" ON "Incidents"("Status", "Severity");
CREATE INDEX IF NOT EXISTS "idx_incidents_response_time" ON "Incidents"("ResponseTimeMinutes");

-- Asset management indexes
CREATE INDEX IF NOT EXISTS "idx_assets_criticality_type" ON "Assets"("Criticality", "AssetType");
CREATE INDEX IF NOT EXISTS "idx_assets_ip_address" ON "Assets"("IPAddress");

-- 11. Create materialized views for performance
CREATE MATERIALIZED VIEW IF NOT EXISTS "mv_threat_actor_intelligence" AS
SELECT 
    ta."ActorName",
    ta."ActorType",
    ta."OriginCountry",
    ta."Sophistication",
    COUNT(DISTINCT tea."ThreatEventId") as "TotalEvents",
    COUNT(DISTINCT te."SourceAddress") as "UniqueIPs",
    COUNT(DISTINCT te."DestinationAddress") as "UniqueTargets",
    AVG(tea."AttributionConfidence") as "AvgConfidence",
    MIN(te."Timestamp") as "FirstActivity",
    MAX(te."Timestamp") as "LastActivity"
FROM "ThreatActors" ta
JOIN "ThreatEventAttribution" tea ON ta."Id" = tea."ActorId"
JOIN "ThreatEvents" te ON tea."ThreatEventId" = te."Id"
WHERE te."DeletedAt" IS NULL
GROUP BY ta."Id", ta."ActorName", ta."ActorType", ta."OriginCountry", ta."Sophistication";

CREATE MATERIALIZED VIEW IF NOT EXISTS "mv_threat_campaign_summary" AS
SELECT 
    tc."CampaignName",
    tc."Status",
    tc."ThreatLevel",
    COUNT(DISTINCT tec."ThreatEventId") as "TotalEvents",
    COUNT(DISTINCT te."SourceAddress") as "UniqueActors",
    COUNT(DISTINCT te."DestinationAddress") as "UniqueTargets",
    tc."FirstSeen",
    tc."LastSeen",
    EXTRACT(EPOCH FROM (tc."LastSeen" - tc."FirstSeen")) / 86400 as "DurationDays"
FROM "ThreatCampaigns" tc
JOIN "ThreatEventCampaigns" tec ON tc."Id" = tec."CampaignId"
JOIN "ThreatEvents" te ON tec."ThreatEventId" = te."Id"
WHERE te."DeletedAt" IS NULL
GROUP BY tc."Id", tc."CampaignName", tc."Status", tc."ThreatLevel", tc."FirstSeen", tc."LastSeen";

-- 12. Create functions for automatic enrichment
CREATE OR REPLACE FUNCTION calculate_risk_score(
    event_count INT,
    unique_ports INT,
    unique_targets INT,
    category TEXT,
    source_country TEXT
) RETURNS DECIMAL(5,2) AS $$
BEGIN
    RETURN LEAST(100.0, (
        -- Base frequency score
        CASE 
            WHEN event_count > 10000 THEN 40
            WHEN event_count > 1000 THEN 25
            WHEN event_count > 100 THEN 15
            ELSE 5
        END +
        -- Category severity
        CASE 
            WHEN category ILIKE '%bot%' OR category ILIKE '%c2%' THEN 30
            WHEN category ILIKE '%malware%' OR category ILIKE '%trojan%' THEN 25
            WHEN category ILIKE '%scan%' OR category ILIKE '%brute%' THEN 20
            ELSE 10
        END +
        -- Targeting diversity
        CASE 
            WHEN unique_ports > 20 THEN 20
            WHEN unique_ports > 10 THEN 15
            WHEN unique_ports > 5 THEN 10
            ELSE 5
        END +
        -- Target diversity
        CASE 
            WHEN unique_targets > 50 THEN 15
            WHEN unique_targets > 10 THEN 10
            WHEN unique_targets > 5 THEN 5
            ELSE 0
        END
    )::DECIMAL(5,2));
END;
$$ LANGUAGE plpgsql;

-- 13. Create refresh procedures for materialized views
CREATE OR REPLACE FUNCTION refresh_threat_intelligence_views()
RETURNS void AS $$
BEGIN
    REFRESH MATERIALIZED VIEW "mv_threat_actor_intelligence";
    REFRESH MATERIALIZED VIEW "mv_threat_campaign_summary";
END;
$$ LANGUAGE plpgsql;

-- Schedule regular refresh (requires pg_cron extension)
-- SELECT cron.schedule('refresh-threat-views', '0 */6 * * *', 'SELECT refresh_threat_intelligence_views();');

COMMENT ON TABLE "ThreatEvents" IS 'Enhanced threat events with comprehensive enrichment data';
COMMENT ON TABLE "ThreatCampaigns" IS 'Attack campaigns and threat actor operations';
COMMENT ON TABLE "ThreatIntelFeeds" IS 'External threat intelligence feed sources';
COMMENT ON TABLE "AttackTechniques" IS 'MITRE ATT&CK framework techniques';
COMMENT ON TABLE "ThreatActors" IS 'Threat actor profiles and attribution';
COMMENT ON TABLE "Assets" IS 'Organizational assets and attack targets';
COMMENT ON TABLE "Incidents" IS 'Security incident management integration';
