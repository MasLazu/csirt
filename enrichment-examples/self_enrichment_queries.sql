-- SQL scripts to enrich your existing ThreatEvents data using patterns and analytics

-- 1. Calculate Risk Scores Based on Your Own Data
UPDATE "ThreatEvents" 
SET "RiskScore" = (
    SELECT LEAST(100, (
        -- Frequency score (more frequent = higher risk)
        (frequency_rank * 20) +
        -- Category severity score
        (CASE 
            WHEN "Category" LIKE '%bot%' OR "Category" LIKE '%c2%' THEN 40
            WHEN "Category" LIKE '%malware%' OR "Category" LIKE '%trojan%' THEN 35
            WHEN "Category" LIKE '%scan%' OR "Category" LIKE '%brute%' THEN 25
            ELSE 15
        END) +
        -- Port targeting score (common attack ports = higher risk)
        (CASE 
            WHEN "DestinationPort" IN (22, 23, 80, 443, 3389, 1433, 3306) THEN 20
            WHEN "DestinationPort" IN (21, 25, 53, 110, 143, 993, 995) THEN 15
            ELSE 5
        END) +
        -- Geographic risk (based on your data patterns)
        (country_risk * 15)
    ))
    FROM (
        SELECT 
            te_inner."SourceAddress",
            PERCENT_RANK() OVER (ORDER BY COUNT(*)) as frequency_rank,
            COALESCE(country_stats.risk_multiplier, 1.0) as country_risk
        FROM "ThreatEvents" te_inner
        LEFT JOIN (
            SELECT 
                "SourceCountryId",
                CASE 
                    WHEN COUNT(*) > 100000 THEN 2.0  -- High-volume countries
                    WHEN COUNT(*) > 50000 THEN 1.5   -- Medium-volume countries
                    ELSE 1.0                         -- Low-volume countries
                END as risk_multiplier
            FROM "ThreatEvents"
            WHERE "DeletedAt" IS NULL
            GROUP BY "SourceCountryId"
        ) country_stats ON te_inner."SourceCountryId" = country_stats."SourceCountryId"
        WHERE te_inner."DeletedAt" IS NULL
        GROUP BY te_inner."SourceAddress", country_stats.risk_multiplier
    ) risk_calc
    WHERE risk_calc."SourceAddress" = "ThreatEvents"."SourceAddress"
)
WHERE "DeletedAt" IS NULL AND "RiskScore" IS NULL;

-- 2. Add Session/Campaign Grouping Based on Temporal Patterns
WITH
    event_sequences AS (
        SELECT
            "SourceAddress",
            "DestinationAddress",
            "Timestamp",
            LAG("Timestamp") OVER (
                PARTITION BY
                    "SourceAddress",
                    "DestinationAddress"
                ORDER BY "Timestamp"
            ) as prev_timestamp,
            ROW_NUMBER() OVER (
                PARTITION BY
                    "SourceAddress",
                    "DestinationAddress"
                ORDER BY "Timestamp"
            ) as seq_num
        FROM "ThreatEvents"
        WHERE
            "DeletedAt" IS NULL
    ),
    session_breaks AS (
        SELECT
            *,
            CASE
                WHEN prev_timestamp IS NULL
                OR "Timestamp" - prev_timestamp > INTERVAL '1 hour' THEN 1
                ELSE 0
            END as is_new_session
        FROM event_sequences
    ),
    sessions AS (
        SELECT *, SUM(is_new_session) OVER (
                PARTITION BY
                    "SourceAddress", "DestinationAddress"
                ORDER BY "Timestamp" ROWS UNBOUNDED PRECEDING
            ) as session_id
        FROM session_breaks
    )
UPDATE "ThreatEvents"
SET
    "SessionId" = gen_random_uuid () -- You'll want to use actual session grouping
WHERE
    "SessionId" IS NULL;

-- 3. Calculate Behavioral Patterns from Your Data
-- Add day of week and hour patterns
UPDATE "ThreatEvents"
SET
    "DayOfWeek" = EXTRACT(
        DOW
        FROM "Timestamp"
    ),
    "HourOfDay" = EXTRACT(
        HOUR
        FROM "Timestamp"
    ),
    "IsWeekend" = EXTRACT(
        DOW
        FROM "Timestamp"
    ) IN (0, 6)
WHERE
    "DayOfWeek" IS NULL;

-- 4. Identify Anomalous Behavior Patterns
WITH hourly_baselines AS (
    SELECT 
        "SourceAddress",
        EXTRACT(HOUR FROM "Timestamp") as hour,
        AVG(hourly_count) as avg_hourly_events,
        STDDEV(hourly_count) as stddev_hourly_events
    FROM (
        SELECT 
            "SourceAddress",
            DATE_TRUNC('hour', "Timestamp") as hour_bucket,
            "Timestamp",
            COUNT(*) OVER (
                PARTITION BY "SourceAddress", DATE_TRUNC('hour', "Timestamp")
            ) as hourly_count
        FROM "ThreatEvents"
        WHERE "DeletedAt" IS NULL 
            AND "Timestamp" > NOW() - INTERVAL '30 days'
    ) hourly_data
    GROUP BY "SourceAddress", EXTRACT(HOUR FROM "Timestamp")
),
current_hour_activity AS (
    SELECT 
        "SourceAddress",
        DATE_TRUNC('hour', "Timestamp") as current_hour,
        COUNT(*) as current_count
    FROM "ThreatEvents"
    WHERE "DeletedAt" IS NULL 
        AND "Timestamp" > NOW() - INTERVAL '1 hour'
    GROUP BY "SourceAddress", DATE_TRUNC('hour', "Timestamp")
)
UPDATE "ThreatEvents" 
SET "AnomalyScore" = (
    SELECT 
        CASE 
            WHEN bl.stddev_hourly_events > 0 THEN
                ABS(cha.current_count - bl.avg_hourly_events) / bl.stddev_hourly_events
            ELSE 0
        END
    FROM hourly_baselines bl
    JOIN current_hour_activity cha ON bl."SourceAddress" = cha."SourceAddress"
    WHERE bl."SourceAddress" = "ThreatEvents"."SourceAddress"
        AND bl.hour = EXTRACT(HOUR FROM "ThreatEvents"."Timestamp")
)
WHERE "AnomalyScore" IS NULL 
    AND "Timestamp" > NOW() - INTERVAL '1 hour';

-- 5. Create Attack Campaign Clusters
WITH ip_behavior_patterns AS (
    SELECT 
        "SourceAddress",
        COUNT(DISTINCT "DestinationPort") as ports_targeted,
        COUNT(DISTINCT "Category") as attack_types,
        COUNT(DISTINCT DATE_TRUNC('day', "Timestamp")) as active_days,
        MIN("Timestamp") as first_seen,
        MAX("Timestamp") as last_seen,
        COUNT(*) as total_events
    FROM "ThreatEvents"
    WHERE "DeletedAt" IS NULL
    GROUP BY "SourceAddress"
),
behavioral_clusters AS (
    SELECT *,
        CASE 
            WHEN ports_targeted > 10 AND attack_types > 3 THEN 'reconnaissance'
            WHEN total_events > 1000 AND active_days > 7 THEN 'persistent_campaign'
            WHEN attack_types = 1 AND total_events > 100 THEN 'focused_attack'
            WHEN active_days = 1 AND total_events > 50 THEN 'burst_attack'
            ELSE 'standard_activity'
        END as behavior_cluster
    FROM ip_behavior_patterns
)
UPDATE "ThreatEvents" 
SET "ClusterGroup" = (
    SELECT behavior_cluster 
    FROM behavioral_clusters 
    WHERE behavioral_clusters."SourceAddress" = "ThreatEvents"."SourceAddress"
)
WHERE "ClusterGroup" IS NULL;