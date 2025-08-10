using Mapster;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatIntelligence.Models;

public class ThreatIntelligenceMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map from TimescaleDB ThreatIntelligence entity to DTO
        config.NewConfig<MeUi.Domain.Entities.ThreatIntelligence, ThreatIntelligenceDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Timestamp, src => src.Timestamp)
            .Map(dest => dest.Asn, src => src.AsnInfo.Asn)
            .Map(dest => dest.AsnInfo, src => src.AsnInfo.Description)
            .Map(dest => dest.SourceAddress, src => src.SourceAddress.ToString())
            .Map(dest => dest.DestinationAddress, src => src.DestinationAddress != null ? src.DestinationAddress.ToString() : null)
            .Map(dest => dest.SourceCountry, src => src.SourceCountry != null ? src.SourceCountry.Code : null)
            .Map(dest => dest.DestinationCountry, src => src.DestinationCountry != null ? src.DestinationCountry.Code : null)
            .Map(dest => dest.SourcePort, src => src.SourcePort)
            .Map(dest => dest.DestinationPort, src => src.DestinationPort)
            .Map(dest => dest.Protocol, src => src.Protocol != null ? src.Protocol.Name : null)
            .Map(dest => dest.Category, src => src.Category)
            .Map(dest => dest.MalwareFamily, src => src.MalwareFamily != null ? src.MalwareFamily.Name : null)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            // Map OptionalInformation for backward compatibility
            .Map(dest => dest.OptionalInformation.DestinationAddress, src => src.DestinationAddress != null ? src.DestinationAddress.ToString() : null)
            .Map(dest => dest.OptionalInformation.DestinationCountry, src => src.DestinationCountry != null ? src.DestinationCountry.Code : null)
            .Map(dest => dest.OptionalInformation.DestinationPort, src => src.DestinationPort != null ? src.DestinationPort.ToString() : null)
            .Map(dest => dest.OptionalInformation.SourcePort, src => src.SourcePort != null ? src.SourcePort.ToString() : null)
            .Map(dest => dest.OptionalInformation.Protocol, src => src.Protocol != null ? src.Protocol.Name : null)
            .Map(dest => dest.OptionalInformation.Family, src => src.MalwareFamily != null ? src.MalwareFamily.Name : null);
    }
}