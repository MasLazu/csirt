namespace MeUi.Application.Models.ThreatGeographic;

public class CountryAsnCorrelationDto
{
    public string Country { get; set; } = string.Empty;
    public string Asn { get; set; } = string.Empty;
    public string AsnDescription { get; set; } = string.Empty;
    public int Events { get; set; }
    public int UniqueIps { get; set; }
    public int Categories { get; set; }
}