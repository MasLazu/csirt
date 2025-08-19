namespace MeUi.Application.Models.ThreatGeographic;

public class CountryAsnCorrelationDto
{
    public string Country { get; set; } = string.Empty;
    public string ASN { get; set; } = string.Empty;
    public string ASNDescription { get; set; } = string.Empty;
    public int Events { get; set; }
    public int UniqueIPs { get; set; }
    public int Categories { get; set; }
}
