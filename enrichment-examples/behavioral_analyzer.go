package main

import (
	"fmt"
	"math"
	"time"
)

// Behavioral Analysis Structures
type BehavioralProfile struct {
	SourceIP            string            `json:"source_ip"`
	TotalEvents         int               `json:"total_events"`
	UniqueTargets       int               `json:"unique_targets"`
	UniqueDestPorts     int               `json:"unique_dest_ports"`
	AttackCategories    []string          `json:"attack_categories"`
	TemporalPattern     map[int]int       `json:"temporal_pattern"` // Hour -> Count
	FrequencyPattern    FrequencyAnalysis `json:"frequency_pattern"`
	AnomalyScore        float64           `json:"anomaly_score"`
	BehaviorCluster     string            `json:"behavior_cluster"`
	ThreatLevel         string            `json:"threat_level"`
	PredictedNextTarget string            `json:"predicted_next_target"`
}

type FrequencyAnalysis struct {
	EventsPerHour    float64 `json:"events_per_hour"`
	EventsPerDay     float64 `json:"events_per_day"`
	StandardDev      float64 `json:"standard_deviation"`
	CoeffVariation   float64 `json:"coefficient_variation"`
	BurstFrequency   float64 `json:"burst_frequency"`
	PatternStability float64 `json:"pattern_stability"`
}

type AnomalyDetection struct {
	ZScore            float64  `json:"z_score"`
	IsolationScore    float64  `json:"isolation_score"`
	ClusterDistance   float64  `json:"cluster_distance"`
	TimeSeriesAnomaly float64  `json:"time_series_anomaly"`
	IsAnomaly         bool     `json:"is_anomaly"`
	AnomalyReasons    []string `json:"anomaly_reasons"`
}

// BehavioralAnalyzer performs ML-style analysis on threat data
type BehavioralAnalyzer struct {
	profiles map[string]*BehavioralProfile
	clusters map[string][]string
}

// NewBehavioralAnalyzer creates a new behavioral analyzer
func NewBehavioralAnalyzer() *BehavioralAnalyzer {
	return &BehavioralAnalyzer{
		profiles: make(map[string]*BehavioralProfile),
		clusters: make(map[string][]string),
	}
}

// AnalyzeIPBehavior analyzes behavioral patterns for an IP address
func (ba *BehavioralAnalyzer) AnalyzeIPBehavior(sourceIP string, events []ThreatEventData) *BehavioralProfile {
	if len(events) == 0 {
		return nil
	}

	profile := &BehavioralProfile{
		SourceIP:         sourceIP,
		TotalEvents:      len(events),
		TemporalPattern:  make(map[int]int),
		AttackCategories: make([]string, 0),
	}

	// Calculate basic statistics
	uniqueTargets := make(map[string]bool)
	uniquePorts := make(map[int]bool)
	categories := make(map[string]bool)

	// Temporal analysis
	hourlyEvents := make(map[int]int)
	dailyEvents := make(map[string]int)

	for _, event := range events {
		// Track unique targets and ports
		uniqueTargets[event.DestinationAddress] = true
		if event.DestinationPort > 0 {
			uniquePorts[event.DestinationPort] = true
		}
		categories[event.Category] = true

		// Temporal patterns
		hour := event.Timestamp.Hour()
		hourlyEvents[hour]++

		day := event.Timestamp.Format("2006-01-02")
		dailyEvents[day]++
	}

	profile.UniqueTargets = len(uniqueTargets)
	profile.UniqueDestPorts = len(uniquePorts)
	profile.TemporalPattern = hourlyEvents

	for category := range categories {
		profile.AttackCategories = append(profile.AttackCategories, category)
	}

	// Frequency analysis
	profile.FrequencyPattern = ba.calculateFrequencyPattern(events, dailyEvents, hourlyEvents)

	// Anomaly detection
	profile.AnomalyScore = ba.calculateAnomalyScore(profile)

	// Behavioral clustering
	profile.BehaviorCluster = ba.classifyBehavior(profile)

	// Threat level assessment
	profile.ThreatLevel = ba.assessThreatLevel(profile)

	// Store profile
	ba.profiles[sourceIP] = profile

	return profile
}

func (ba *BehavioralAnalyzer) calculateFrequencyPattern(events []ThreatEventData, dailyEvents map[string]int, hourlyEvents map[int]int) FrequencyAnalysis {
	// Calculate events per time unit
	totalDays := float64(len(dailyEvents))
	if totalDays == 0 {
		totalDays = 1
	}

	eventsPerDay := float64(len(events)) / totalDays
	eventsPerHour := eventsPerDay / 24

	// Calculate standard deviation for daily events
	var dailyCounts []float64
	for _, count := range dailyEvents {
		dailyCounts = append(dailyCounts, float64(count))
	}

	stdDev := calculateStandardDeviation(dailyCounts)
	coeffVar := 0.0
	if eventsPerDay > 0 {
		coeffVar = stdDev / eventsPerDay
	}

	// Calculate burst frequency (events above 2 standard deviations)
	burstThreshold := eventsPerDay + (2 * stdDev)
	burstDays := 0
	for _, count := range dailyCounts {
		if count > burstThreshold {
			burstDays++
		}
	}
	burstFrequency := float64(burstDays) / totalDays

	// Pattern stability (inverse of coefficient of variation)
	patternStability := 1.0 / (1.0 + coeffVar)

	return FrequencyAnalysis{
		EventsPerHour:    eventsPerHour,
		EventsPerDay:     eventsPerDay,
		StandardDev:      stdDev,
		CoeffVariation:   coeffVar,
		BurstFrequency:   burstFrequency,
		PatternStability: patternStability,
	}
}

func (ba *BehavioralAnalyzer) calculateAnomalyScore(profile *BehavioralProfile) float64 {
	score := 0.0

	// Volume anomaly (Z-score based on all profiles)
	if len(ba.profiles) > 1 {
		allEventCounts := make([]float64, 0, len(ba.profiles))
		for _, p := range ba.profiles {
			allEventCounts = append(allEventCounts, float64(p.TotalEvents))
		}

		mean := calculateMean(allEventCounts)
		stdDev := calculateStandardDeviation(allEventCounts)

		if stdDev > 0 {
			zScore := math.Abs(float64(profile.TotalEvents)-mean) / stdDev
			score += zScore * 0.3 // 30% weight for volume anomaly
		}
	}

	// Diversity anomaly (unusual port/target diversity)
	if profile.TotalEvents > 0 {
		portDiversity := float64(profile.UniqueDestPorts) / float64(profile.TotalEvents)
		targetDiversity := float64(profile.UniqueTargets) / float64(profile.TotalEvents)

		// High diversity suggests scanning behavior
		if portDiversity > 0.1 || targetDiversity > 0.1 {
			score += (portDiversity + targetDiversity) * 50 // Amplify diversity score
		}
	}

	// Temporal anomaly (unusual time patterns)
	temporalEntropy := ba.calculateTemporalEntropy(profile.TemporalPattern)
	if temporalEntropy > 3.0 { // High entropy = distributed across many hours
		score += temporalEntropy * 0.2
	}

	// Frequency pattern anomalies
	if profile.FrequencyPattern.BurstFrequency > 0.3 { // Frequent bursts
		score += profile.FrequencyPattern.BurstFrequency * 30
	}

	if profile.FrequencyPattern.CoeffVariation > 2.0 { // Very irregular pattern
		score += profile.FrequencyPattern.CoeffVariation * 5
	}

	return math.Min(score, 100.0) // Cap at 100
}

func (ba *BehavioralAnalyzer) calculateTemporalEntropy(hourlyPattern map[int]int) float64 {
	total := 0
	for _, count := range hourlyPattern {
		total += count
	}

	if total == 0 {
		return 0
	}

	entropy := 0.0
	for _, count := range hourlyPattern {
		if count > 0 {
			probability := float64(count) / float64(total)
			entropy -= probability * math.Log2(probability)
		}
	}

	return entropy
}

func (ba *BehavioralAnalyzer) classifyBehavior(profile *BehavioralProfile) string {
	// Simple rule-based clustering

	// Scanner behavior
	if profile.UniqueDestPorts > 10 && profile.UniqueTargets > 5 {
		return "scanner"
	}

	// Persistent attacker
	if profile.TotalEvents > 1000 && profile.FrequencyPattern.PatternStability > 0.7 {
		return "persistent_attacker"
	}

	// Burst attacker
	if profile.FrequencyPattern.BurstFrequency > 0.5 {
		return "burst_attacker"
	}

	// Focused attacker
	if profile.UniqueTargets <= 2 && profile.TotalEvents > 100 {
		return "focused_attacker"
	}

	// Bot-like behavior
	if profile.FrequencyPattern.PatternStability > 0.8 && profile.TotalEvents > 50 {
		return "bot_like"
	}

	// Reconnaissance
	if len(profile.AttackCategories) > 3 && profile.UniqueDestPorts > 5 {
		return "reconnaissance"
	}

	return "standard_activity"
}

func (ba *BehavioralAnalyzer) assessThreatLevel(profile *BehavioralProfile) string {
	score := 0

	// Volume scoring
	if profile.TotalEvents > 10000 {
		score += 40
	} else if profile.TotalEvents > 1000 {
		score += 25
	} else if profile.TotalEvents > 100 {
		score += 15
	}

	// Diversity scoring
	if profile.UniqueDestPorts > 20 {
		score += 30
	} else if profile.UniqueDestPorts > 10 {
		score += 20
	}

	if profile.UniqueTargets > 50 {
		score += 25
	} else if profile.UniqueTargets > 10 {
		score += 15
	}

	// Anomaly scoring
	if profile.AnomalyScore > 50 {
		score += 20
	} else if profile.AnomalyScore > 25 {
		score += 10
	}

	// Behavioral pattern scoring
	switch profile.BehaviorCluster {
	case "scanner", "reconnaissance":
		score += 25
	case "persistent_attacker":
		score += 35
	case "burst_attacker":
		score += 20
	case "bot_like":
		score += 15
	}

	// Threat level classification
	if score >= 80 {
		return "critical"
	} else if score >= 60 {
		return "high"
	} else if score >= 40 {
		return "medium"
	} else if score >= 20 {
		return "low"
	}

	return "minimal"
}

// Helper functions
func calculateMean(values []float64) float64 {
	if len(values) == 0 {
		return 0
	}

	sum := 0.0
	for _, v := range values {
		sum += v
	}
	return sum / float64(len(values))
}

func calculateStandardDeviation(values []float64) float64 {
	if len(values) == 0 {
		return 0
	}

	mean := calculateMean(values)
	sumSquaredDiffs := 0.0

	for _, v := range values {
		diff := v - mean
		sumSquaredDiffs += diff * diff
	}

	variance := sumSquaredDiffs / float64(len(values))
	return math.Sqrt(variance)
}

// ThreatEventData represents a simplified threat event for analysis
type ThreatEventData struct {
	SourceAddress      string
	DestinationAddress string
	DestinationPort    int
	Category           string
	Timestamp          time.Time
}

// Example usage for behavioral analysis
func AnalyzeThreatEventBehaviors() {
	analyzer := NewBehavioralAnalyzer()

	// Sample data (you'd load this from your database)
	sampleEvents := map[string][]ThreatEventData{
		"192.168.1.100": {
			{SourceAddress: "192.168.1.100", DestinationAddress: "10.0.0.1", DestinationPort: 22, Category: "brute", Timestamp: time.Now().Add(-time.Hour)},
			{SourceAddress: "192.168.1.100", DestinationAddress: "10.0.0.2", DestinationPort: 22, Category: "brute", Timestamp: time.Now().Add(-30 * time.Minute)},
			{SourceAddress: "192.168.1.100", DestinationAddress: "10.0.0.3", DestinationPort: 22, Category: "brute", Timestamp: time.Now()},
		},
	}

	// Analyze each IP's behavior
	for sourceIP, events := range sampleEvents {
		profile := analyzer.AnalyzeIPBehavior(sourceIP, events)
		if profile != nil {
			fmt.Printf("\nBehavioral Analysis for IP: %s\n", sourceIP)
			fmt.Printf("Total Events: %d\n", profile.TotalEvents)
			fmt.Printf("Unique Targets: %d\n", profile.UniqueTargets)
			fmt.Printf("Unique Ports: %d\n", profile.UniqueDestPorts)
			fmt.Printf("Behavior Cluster: %s\n", profile.BehaviorCluster)
			fmt.Printf("Threat Level: %s\n", profile.ThreatLevel)
			fmt.Printf("Anomaly Score: %.2f\n", profile.AnomalyScore)
			fmt.Printf("Attack Categories: %v\n", profile.AttackCategories)

			// Store results in database
			updateSQL := `
				UPDATE "ThreatEvents" 
				SET "AnomalyScore" = $1, "ClusterGroup" = $2, "ThreatLevel" = $3,
					"BehaviorProfile" = $4
				WHERE "SourceAddress" = $5`

			// profileJSON, _ := json.Marshal(profile)
			// db.Exec(updateSQL, profile.AnomalyScore, profile.BehaviorCluster, profile.ThreatLevel, profileJSON, sourceIP)
		}
	}
}
