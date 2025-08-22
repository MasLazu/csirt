namespace MeUi.Application.Models.ThreatGeographic;

public class CrossBorderAttackFlowDto
{
    public string SourceCountry { get; set; } = string.Empty;
    public string DestinationCountry { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Events { get; set; }
}