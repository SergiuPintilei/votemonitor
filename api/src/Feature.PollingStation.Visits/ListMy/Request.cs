﻿using Vote.Monitor.Core.Security;

namespace Feature.PollingStation.Visits.ListMy;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}
