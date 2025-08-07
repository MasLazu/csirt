namespace MeUi.Domain.Entities;

public class OptionalInformation
{
    public string? DestinationAddress { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DestinationPort { get; set; }
    public string? SourcePort { get; set; }
    public string? Protocol { get; set; }
    public string? Family { get; set; }
}