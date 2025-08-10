using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Exceptions;
using MeUi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MeUi.Application.Features.ThreatIntelligence.Models;

namespace MeUi.Application.Features.ThreatIntelligence.Queries.GetThreatRelationshipGraph;

public class GetThreatRelationshipGraphQueryHandler : IRequestHandler<GetThreatRelationshipGraphQuery, ThreatRelationshipGraphDto>
{
    private readonly IThreatEventRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetThreatRelationshipGraphQueryHandler(
        IThreatEventRepository repository,
        ITenantContext tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ThreatRelationshipGraphDto> Handle(GetThreatRelationshipGraphQuery request, CancellationToken ct)
    {
        // Validate tenant context - only authenticated users with valid tenant context can access threat intelligence
        if (!_tenantContext.IsSuperAdmin && !_tenantContext.TenantId.HasValue)
        {
            throw new TenantAccessDeniedException();
        }

        try
        {
            var startDateUtc = request.StartDate.Kind == DateTimeKind.Utc
                ? request.StartDate
                : request.StartDate.ToUniversalTime();
            var endDateUtc = request.EndDate.Kind == DateTimeKind.Utc
                ? request.EndDate
                : request.EndDate.ToUniversalTime();

            // Get ASN-Country relationships
            var asnCountryData = await _repository.GetThreatCountsByAsnAsync(startDateUtc, endDateUtc, request.MaxNodes / 2, ct);
            var countryData = await _repository.GetThreatCountsByCountryAsync(startDateUtc, endDateUtc, request.MaxNodes / 2, ct);

            var nodes = new List<GraphNodeDto>();
            var edges = new List<GraphEdgeDto>();

            // Create ASN nodes
            foreach (var asn in asnCountryData.Take(10))
            {
                nodes.Add(new GraphNodeDto
                {
                    Id = $"asn:{asn.Asn}",
                    Label = $"{asn.Asn} - {asn.AsnDescription}",
                    Type = "asn",
                    ThreatCount = asn.ThreatCount,
                    Size = Math.Min(50, Math.Max(10, asn.ThreatCount / 10)),
                    Color = "#FF6B6B"
                });
            }

            // Create Country nodes
            foreach (var country in countryData.Take(10))
            {
                nodes.Add(new GraphNodeDto
                {
                    Id = $"country:{country.CountryCode}",
                    Label = country.CountryName,
                    Type = "country",
                    ThreatCount = country.ThreatCount,
                    Size = Math.Min(50, Math.Max(10, country.ThreatCount / 10)),
                    Color = "#4ECDC4"
                });
            }

            // Create simple edges (mock relationships)
            var asnNodes = nodes.Where(n => n.Type == "asn").Take(5);
            var countryNodes = nodes.Where(n => n.Type == "country").Take(5);

            int edgeId = 0;
            foreach (var asn in asnNodes)
            {
                var country = countryNodes.Skip(edgeId % countryNodes.Count()).First();
                edges.Add(new GraphEdgeDto
                {
                    Id = $"edge_{edgeId++}",
                    SourceId = asn.Id,
                    TargetId = country.Id,
                    RelationType = "originates_from",
                    Weight = Math.Min(asn.ThreatCount, country.ThreatCount),
                    Thickness = Math.Min(8, Math.Max(1, Math.Min(asn.ThreatCount, country.ThreatCount) / 100))
                });
            }

            return new ThreatRelationshipGraphDto
            {
                Nodes = nodes,
                Edges = edges,
                TotalNodes = nodes.Count,
                TotalEdges = edges.Count
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving threat relationship graph: {ex.Message}", ex);
        }
    }
}