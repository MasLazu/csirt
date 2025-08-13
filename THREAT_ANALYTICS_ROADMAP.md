# Threat Analytics Roadmap & Task Tracker

Status legend: ✅ done · 🚧 in progress / partial · ⏳ planned / not started · 🔍 needs data model support

## P1 Core (User-Facing Now)

- ✅ Timeline analytics (`/api/v1/threat-events/analytics/timeline`) – per-bucket malware added (severity deferred)
- ✅ Summary metrics (repository) – exposed `/overview` endpoint ✅
- ✅ Top categories (`/categories/top`) – public endpoint exposure (🚧 verify route)
- ✅ Top malware families (`/malware-families/top`) – public endpoint exposure (🚧)
- ✅ IP reputation top (`/ip-reputation/top`)
- ✅ Port analytics (`/ports/top`) – source/destination support
- ✅ Dashboard metrics endpoint – basic metrics delivered (extend with peak hour, active threats refinement later)

## P2 Distribution & Geo

- ✅ Protocol distribution (`/protocols/distribution`)
- ✅ Geo heatmap (`/geo/heatmap`)
- ✅ Top ASNs (`/asns/top`)
- ✅ Source countries top (`/countries/source/top`)
- 🔍 Destination countries top (`/countries/destination/top`) – needs destination geo data
- ✅ Category timeline (`/categories/timeline`)
- ✅ Malware family timeline (`/malware-families/timeline`)
- ✅ Comparative timeline (`/timeline/comparative`)

## P3 Advanced Intelligence

- ✅ Anomalies base (`/anomalies`) – refine scoring & params (🚧)
- ✅ Correlations base (`/correlations`) – add lift/strength metrics (🚧)
- ⏳ Trending (`/trending?dimension=category|malwareFamily`)
- ⏳ Entity detail endpoints (`/categories/{id}/detail`, `/malware-families/{id}/detail`)
- ⏳ Risk score distribution (`/risk-score/distribution`) – 🔍 real RiskScore column required

## P4 Deep Insights & Ops

proceed

- ⏳ Cooccurrence matrix (`/cooccurrence?entityA=category&entityB=malwareFamily`)
- ⏳ Spikes detection (`/spikes?dimension=category|port`)
- ⏳ Geo flows (`/geo/flows`) – 🔍 destination country data required
- ⏳ Ingest latency (`/ingest/latency`) – needs ingest timestamp
- ⏳ Data gaps (`/data-gaps`) – detect missing buckets
- ⏳ Kill chain stages (`/killchain/stages`) – needs classification field

## Cross-Cutting Enhancements

- ⏳ Multi-tenancy enforcement (remove tenantId:null placeholders)
- ⏳ Caching layer (short TTL) for heavy queries (heatmap, correlations)
- ⏳ Composite indexes: `(Timestamp, Category)`, `(Timestamp, MalwareFamilyId)`, `(Timestamp, SourceCountryId)`, `(Timestamp, ProtocolId)`, `(Timestamp, DestinationPort)`
- 🔍 Add `RiskScore` column + backfill (stop hardcoding)
- ⏳ Destination geo fields (`DestinationCountryId`) for flows/targeted metrics

## Data Model / SQL Enhancements Needed

| Need                 | Purpose                                    | Status |
| -------------------- | ------------------------------------------ | ------ |
| RiskScore column     | Real risk distribution & anomaly weighting | 🔍     |
| DestinationCountryId | Geo flows & targeted country stats         | 🔍     |
| Composite indexes    | Performance for analytic grouping          | ⏳     |
| IngestTimestamp      | Latency & data gap detection               | ⏳     |

## Next Implementation Suggestions

1. Confirm public routes for existing top endpoints & add Swagger examples.
2. (Done) Timeline malware per bucket; severity deferred until field exists.
3. Implement protocol distribution & geo heatmap queries (server aggregation).
4. Add trending (dual-window CTE) and real risk score storage.
5. Introduce caching for expensive endpoints.

## Acceptance Criteria (Phase Examples)

**P1:** 95th percentile response < 1.2s for 30-day window; bucket sum ≈ summary total (±1%); percentages sum ≤ 100%.
**P2:** Geo heatmap latency < 1.5s; protocol distribution < 500ms.
**P3:** Trending reflects correct delta vs prior window; anomalies >80% precision on test set.

## Testing Strategy

- Seed deterministic dataset for integration tests.
- Add repository tests for each new aggregate.
- Use Testcontainers (PostgreSQL) for analytics integration tests.

## Change Log (Analytics)

| Date       | Change                                                  | Status |
| ---------- | ------------------------------------------------------- | ------ |
| 2025-08-13 | Added roadmap & parallelized summary handler            | ✅     |
| 2025-08-13 | Fixed timeline duplication (bucket grouping)            | ✅     |
| 2025-08-13 | Added overview analytics endpoint                       | ✅     |
| 2025-08-13 | Added protocol distribution & geo heatmap               | ✅     |
| 2025-08-13 | Added trend metrics to category timeline endpoint       | ✅     |
| 2025-08-13 | Added trend metrics to malware family timeline endpoint | ✅     |

_Update this file as features progress; keep ✅ entries for release notes._
