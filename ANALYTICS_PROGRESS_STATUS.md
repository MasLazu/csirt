# Analytics Implementation Progress

Last Updated: 2025-08-13 UTC

Legend: ✅ Done · 🚧 In Progress / Partial · ⏳ Not Started / Planned · 🔍 Blocked (data/model) · ♻️ Refactor / Enhancement Later

---

## 1. Core Aggregation Endpoints

| Feature                 | Global Endpoint                                               | Tenant Endpoint (planned path)                                                  | Status | Notes                                                    |
| ----------------------- | ------------------------------------------------------------- | ------------------------------------------------------------------------------- | ------ | -------------------------------------------------------- |
| Timeline (generic)      | GET /api/v1/threat-events/analytics/timeline                  | GET /api/v1/tenant/{tenantId}/threat-events/analytics/timeline                  | ✅     | Tenant endpoint added                                    |
| Comparative Timeline    | GET /api/v1/threat-events/analytics/timeline/comparative      | GET /api/v1/tenant/{tenantId}/threat-events/analytics/timeline/comparative      | ✅     | Tenant endpoint added                                    |
| Category Timeline       | GET /api/v1/threat-events/analytics/categories/timeline       | GET /api/v1/tenant/{tenantId}/threat-events/analytics/categories/timeline       | ✅     | Tenant added; per-bucket trend placeholder (\_trend_pct) |
| Malware Family Timeline | GET /api/v1/threat-events/analytics/malware-families/timeline | GET /api/v1/tenant/{tenantId}/threat-events/analytics/malware-families/timeline | ✅     | Tenant added; per-bucket trend placeholder (\_trend_pct) |
| Overview                | GET /api/v1/threat-events/analytics/overview                  | GET /api/v1/tenant/{tenantId}/threat-events/analytics/overview                  | ✅     |                                                          |
| Summary                 | GET /api/v1/threat-events/analytics/summary                   | GET /api/v1/tenant/{tenantId}/threat-events/analytics/summary                   | ✅     |                                                          |
| Dashboard Metrics       | GET /api/v1/threat-events/analytics/dashboard                 | GET /api/v1/tenant/{tenantId}/threat-events/analytics/dashboard                 | ✅     | Basic metrics only                                       |

## 2. Distribution & Top Lists

| Feature                    | Global Endpoint                                           | Tenant Endpoint (planned path)                                              | Status      | Notes                                 |
| -------------------------- | --------------------------------------------------------- | --------------------------------------------------------------------------- | ----------- | ------------------------------------- |
| Protocol Distribution      | /api/v1/threat-events/analytics/protocols/distribution    | /api/v1/tenant/{tenantId}/threat-events/analytics/protocols/distribution    | ✅          |                                       |
| Geo Heatmap                | /api/v1/threat-events/analytics/geo/heatmap               | /api/v1/tenant/{tenantId}/threat-events/analytics/geo/heatmap               | ✅          |                                       |
| Top ASNs                   | /api/v1/threat-events/analytics/asns/top                  | /api/v1/tenant/{tenantId}/threat-events/analytics/asns/top                  | ✅          |                                       |
| Source Countries Top       | /api/v1/threat-events/analytics/countries/source/top      | /api/v1/tenant/{tenantId}/threat-events/analytics/countries/source/top      | ✅          |                                       |
| Destination Countries Top  | /api/v1/threat-events/analytics/countries/destination/top | /api/v1/tenant/{tenantId}/threat-events/analytics/countries/destination/top | 🔍          | Needs destination geo data            |
| Ports (source/destination) | /api/v1/threat-events/analytics/ports/top                 | /api/v1/tenant/{tenantId}/threat-events/analytics/ports/top                 | ✅ (global) | Global endpoint added; tenant pending |
| IP Reputation Top          | /api/v1/threat-events/analytics/ip-reputation/top         | /api/v1/tenant/{tenantId}/threat-events/analytics/ip-reputation/top         | ✅ (global) | Global endpoint added; tenant pending |

## 3. Advanced Intelligence

| Feature                          | Endpoint (global)                                       | Status                                   | Tenant Adaptation         | Notes                                   |
| -------------------------------- | ------------------------------------------------------- | ---------------------------------------- | ------------------------- | --------------------------------------- | ------------------------------ |
| Anomalies                        | /api/v1/threat-events/analytics/anomalies (future)      | ⏳                                       | Filter by tenant subquery | Base models exist                       |
| Correlations                     | /api/v1/threat-events/analytics/correlations (future)   | ⏳                                       | Same                      | Need final schema for correlation types |
| Trending (dimension=category     | malwareFamily)                                          | /api/v1/threat-events/analytics/trending | ⏳                        | Same                                    | Will reuse dual-window pattern |
| Entity Detail (category/malware) | /api/v1/threat-events/analytics/categories/{id}/detail  | ⏳                                       | Same                      | Requires composite aggregates           |
| Risk Score Distribution          | /api/v1/threat-events/analytics/risk-score/distribution | 🔍                                       | Same                      | Needs RiskScore column                  |
| Cooccurrence Matrix              | /api/v1/threat-events/analytics/cooccurrence            | ⏳                                       | Same                      |                                         |
| Spikes Detection                 | /api/v1/threat-events/analytics/spikes                  | ⏳                                       | Same                      |                                         |
| Geo Flows                        | /api/v1/threat-events/analytics/geo/flows               | 🔍                                       | Same                      | Needs destination data                  |
| Data Gaps                        | /api/v1/threat-events/analytics/data-gaps               | ⏳                                       | Same                      | Needs ingest timestamp                  |
| Kill Chain Stages                | /api/v1/threat-events/analytics/killchain/stages        | 🔍                                       | Same                      | Needs classification field              |

## 4. Cross-Cutting / Multi-Tenancy Tasks

| Task                                                    | Status | Notes                                                                          |
| ------------------------------------------------------- | ------ | ------------------------------------------------------------------------------ |
| Add TenantId to all query DTOs                          | ✅     | Added to all existing analytics queries (ports/ip reputation pending creation) |
| Duplicate endpoints under /api/v1/tenant/{tenantId}/... | ✅     | All core analytics tenant endpoints added                                      |
| Repository tenant filter review                         | ✅     | Added missing previous-period category tenant filter; audited core methods     |
| Remove fallback to global when tenant provided          | ⏳     | Enforce strict scoping                                                         |
| Composite index (AsnRegistryId, Timestamp)              | ⏳     | Migration needed                                                               |
| Index TenantAsnRegistries (TenantId, AsnRegistryId)     | ⏳     | Migration needed                                                               |
| Roadmap update for multi-tenancy basic                  | ⏳     | After endpoint duplication                                                     |

## 5. Technical Debt / Enhancements

| Item                                                          | Status | Notes                                  |
| ------------------------------------------------------------- | ------ | -------------------------------------- |
| Replace Severity dict trend placeholders with explicit fields | ♻️     | Introduce TimelineBucketTrendDto       |
| Consolidate trend calc logic (DRY)                            | ♻️     | Utility service or base handler        |
| Unified validator for time range (30d max)                    | ♻️     | Current duplication                    |
| Add caching (protocol, geo heatmap)                           | ⏳     | After correctness locked               |
| Add integration tests with seeded dataset                     | ⏳     | Deferred per instruction               |
| RiskScore real computation + storage                          | 🔍     | Placeholder constant 5.0 / 7.5 in code |

## 6. Immediate Next Steps (Updated)

1. Enforce strict scoping (reject tenant routes without TenantId; remove permissive fallbacks).
2. Add performance indexes:
   - (AsnRegistryId, Timestamp) on ThreatEvents (btree)
   - (TenantId, AsnRegistryId) on TenantAsnRegistries
3. Expose Ports & IP Reputation endpoints (global + tenant) using existing repo methods.
4. Refactor trend placeholders (\_trend_pct) into TimelineBucketTrendDto; remove magic key.
5. Add unified time range validator & reuse across handlers.
6. Update THREAT_ANALYTICS_ROADMAP.md to mark multi-tenancy base complete.
7. (Optional) Add simple in-memory caching for protocol & geo heatmap (short TTL) post correctness.

## 7. Completed Trend Work

| Item                                     | Status | Notes                            |
| ---------------------------------------- | ------ | -------------------------------- |
| Category timeline per-period trend       | ✅     | Stored in Severity["_trend_pct"] |
| Malware family timeline per-period trend | ✅     | Stored in Severity["_trend_pct"] |

---

_This file is a living status tracker; update after each feature or structural change._
