# Threat Event Analytics Implementation Plan

## Overview

This document outlines the comprehensive analytics implementation for ThreatEvent management, designed to provide rich threat intelligence capabilities for frontend dashboards.

## 🎯 Key Design Principles

### 1. **TimescaleDB Optimization**

- Leverage time-bucketing functions (`time_bucket()`)
- Use continuous aggregates for performance
- Optimize queries with time-range partitioning
- Implement efficient indexing strategies

### 2. **Multi-Tenant Architecture**

- Tenant-scoped analytics based on ASN registries
- Data isolation and security
- Configurable access levels
- Role-based analytics permissions

### 3. **Real-Time Performance**

- Caching frequently accessed aggregations
- Efficient query patterns for large datasets (25M+ records)
- Background pre-computation for complex analytics
- Pagination and streaming for large result sets

## 📊 Analytics Endpoints Specification

### 1. Timeline Analytics

**Endpoint:** `GET /api/v1/threat-events/analytics/timeline`

**Query Parameters:**

- `startTime` / `endTime`: Time range filtering
- `interval`: hour, day, week, month
- `category`: Filter by threat category
- `malwareFamilyId`: Filter by malware family
- `sourceCountryId` / `destinationCountryId`: Geographic filtering
- `tenantId`: Tenant scoping

**Response Structure:**

```json
{
  "timeline": [
    {
      "timestamp": "2025-08-13T00:00:00Z",
      "eventCount": 1250,
      "categories": {
        "malware": 450,
        "phishing": 300,
        "ddos": 500
      },
      "severity": {
        "critical": 150,
        "high": 400,
        "medium": 500,
        "low": 200
      }
    }
  ],
  "totalEvents": 125000,
  "timeRange": {
    "start": "2025-08-01T00:00:00Z",
    "end": "2025-08-13T23:59:59Z"
  },
  "interval": "day"
}
```

### 2. Geographic Distribution Analytics

**Endpoint:** `GET /api/v1/threat-events/analytics/geo-distribution`

**Response Structure:**

```json
{
  "sourceCountries": [
    {
      "countryId": "uuid",
      "countryName": "United States",
      "countryCode": "US",
      "eventCount": 15000,
      "percentage": 25.5,
      "topCategories": ["malware", "phishing"]
    }
  ],
  "destinationCountries": [...],
  "threatFlows": [
    {
      "sourceCountry": "CN",
      "destinationCountry": "US",
      "eventCount": 5000,
      "primaryCategory": "malware"
    }
  ],
  "totalUniqueCountries": 145
}
```

### 3. Malware Trends Analytics

**Endpoint:** `GET /api/v1/threat-events/analytics/malware-trends`

**Response Structure:**

```json
{
  "topMalwareFamilies": [
    {
      "malwareFamilyId": "uuid",
      "name": "Zeus",
      "eventCount": 8500,
      "percentage": 15.2,
      "trend": "increasing",
      "firstSeen": "2025-07-01T00:00:00Z",
      "lastSeen": "2025-08-13T12:00:00Z",
      "geographicSpread": ["US", "DE", "JP"]
    }
  ],
  "emergingThreats": [...],
  "malwareTimeline": [
    {
      "timestamp": "2025-08-13T00:00:00Z",
      "malwareFamilies": {
        "Zeus": 150,
        "Emotet": 100,
        "TrickBot": 75
      }
    }
  ]
}
```

### 4. Network Intelligence Analytics

**Endpoint:** `GET /api/v1/threat-events/analytics/network-intelligence`

**Response Structure:**

```json
{
  "topSourceIPs": [
    {
      "ipAddress": "192.168.1.100",
      "eventCount": 2500,
      "asnRegistry": "AS12345",
      "country": "CN",
      "threatCategories": ["malware", "botnet"],
      "confidence": 0.95
    }
  ],
  "portAnalysis": {
    "topSourcePorts": [
      {"port": 80, "count": 15000, "protocols": ["HTTP"]},
      {"port": 443, "count": 12000, "protocols": ["HTTPS"]}
    ],
    "topDestinationPorts": [...]
  },
  "protocolDistribution": {
    "TCP": 65.5,
    "UDP": 25.2,
    "ICMP": 9.3
  },
  "asnAnalysis": [
    {
      "asnRegistryId": "uuid",
      "asn": "AS12345",
      "organizationName": "Example ISP",
      "eventCount": 5000,
      "threatRatio": 0.85
    }
  ]
}
```

### 5. Threat Summary Dashboard

**Endpoint:** `GET /api/v1/threat-events/analytics/summary`

**Response Structure:**

```json
{
  "overview": {
    "totalEvents": 1250000,
    "last24Hours": 15000,
    "last7Days": 95000,
    "last30Days": 420000
  },
  "trendIndicators": {
    "dailyChange": 5.2,
    "weeklyChange": -2.1,
    "monthlyChange": 15.8
  },
  "threatCategories": {
    "malware": { "count": 450000, "percentage": 36.0 },
    "phishing": { "count": 300000, "percentage": 24.0 },
    "ddos": { "count": 250000, "percentage": 20.0 },
    "botnet": { "count": 150000, "percentage": 12.0 },
    "other": { "count": 100000, "percentage": 8.0 }
  },
  "criticalAlerts": [
    {
      "type": "spike_detected",
      "message": "300% increase in malware events from China",
      "timestamp": "2025-08-13T10:00:00Z",
      "severity": "high"
    }
  ],
  "topThreats": {
    "malwareFamilies": [
      { "name": "Zeus", "count": 5000 },
      { "name": "Emotet", "count": 3500 }
    ],
    "sourceCountries": [
      { "name": "China", "count": 25000 },
      { "name": "Russia", "count": 18000 }
    ]
  }
}
```

## 🏗️ Implementation Strategy

### Phase 1: Core Analytics (Current Sprint)

1. ✅ Timeline Analytics
2. ✅ Geographic Distribution
3. ✅ Threat Summary Dashboard

### Phase 2: Advanced Analytics (Next Sprint)

1. Malware Trends Analysis
2. Network Intelligence
3. Performance optimization with caching

### Phase 3: Intelligence Features (Future)

1. Anomaly detection
2. Correlation analysis
3. Predictive analytics
4. Threat actor profiling

## 🔧 Technical Implementation

### Database Optimization

- Create TimescaleDB continuous aggregates for common time intervals
- Implement materialized views for frequently accessed analytics
- Use proper indexing on timestamp, AsnRegistryId, and filtering columns

### Caching Strategy

- Redis caching for dashboard summaries (TTL: 5-15 minutes)
- Background refresh for expensive aggregations
- Cache invalidation on new threat event ingestion

### Performance Considerations

- Implement query timeout limits
- Use connection pooling
- Async processing for complex analytics
- Result pagination for large datasets

### Security & Access Control

- Tenant-based data filtering
- Analytics permission levels (READ:ANALYTICS, READ:TENANT_ANALYTICS)
- API rate limiting for analytics endpoints
- Data anonymization options

## 📈 Dashboard Integration Points

### Frontend Dashboard Components

1. **Executive Summary Cards** → Summary endpoint
2. **Time-Series Charts** → Timeline endpoint
3. **Geographic Heat Maps** → Geo-distribution endpoint
4. **Threat Category Pie Charts** → Summary + Timeline endpoints
5. **Top Lists & Rankings** → All analytics endpoints
6. **Real-time Alerts** → Summary critical alerts
7. **Drill-down Tables** → Filtered analytics queries

### Real-Time Updates

- WebSocket integration for live dashboard updates
- Event-driven notifications for critical threat spikes
- Configurable refresh intervals per dashboard component

This analytics implementation will provide comprehensive threat intelligence capabilities while maintaining optimal performance for large-scale time-series data analysis.
