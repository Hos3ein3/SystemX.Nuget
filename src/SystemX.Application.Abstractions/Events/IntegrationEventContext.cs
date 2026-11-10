namespace SystemX.Application.Abstractions.Events;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides metadata and transport context for a received integration event.
/// </summary>
/// <param name="MessageId">Unique identifier of the message.</param>
/// <param name="CorrelationId">Identifier correlating related operations or requests across services.</param>
/// <param name="CausationId">Identifier of the upstream message or action that directly caused this event.</param>
/// <param name="TenantId">Tenant identifier if the message is tenant-scoped.</param>
/// <param name="OccurredOn">Timestamp indicating when the event originally occurred.</param>
/// <param name="Headers">Transport and application headers associated with the message.</param>
public sealed record IntegrationEventContext(
    Guid MessageId,
    string? CorrelationId,
    string? CausationId,
    string? TenantId,
    DateTimeOffset OccurredOn,
    IReadOnlyDictionary<string, string> Headers);
