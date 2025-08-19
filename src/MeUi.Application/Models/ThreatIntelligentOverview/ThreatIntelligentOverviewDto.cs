using System;
using System.Collections.Generic;

namespace MeUi.Application.Models.ThreatIntelligentOverview
{
    public class ThreatIntelligentOverviewDto
    {
        public List<ExecutiveSummaryMetricDto> ExecutiveSummary { get; set; } = new();
        public List<TimelineDataPointDto> ThreatActivityTimeline { get; set; } = new();
        public List<TopCategoryDto> TopThreatCategories { get; set; } = new();
        public List<TopCountryDto> TopSourceCountries { get; set; } = new();
        public List<ProtocolDistributionDto> ProtocolDistribution { get; set; } = new();
        public List<HighRiskSourceIpDto> HighRiskSourceIps { get; set; } = new();
        public List<TargetedPortDto> TopTargetedPorts { get; set; } = new();
        public List<ThreatCategoryAnalysisDto> ThreatCategoryAnalysis { get; set; } = new();
    }
}
