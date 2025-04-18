﻿using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
    public QuickReportLocationType QuickReportLocationType { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid? PollingStationId { set; get; }
    public string? PollingStationDetails { get; set; }

    public IncidentCategory IncidentCategory { get; set; } = IncidentCategory.Other;
    
    /// <summary>
    /// Temporary made nullable until we release a mobile version that will always send this property.
    /// </summary>
    public DateTime? LastUpdatedAt { get; set; }
}
