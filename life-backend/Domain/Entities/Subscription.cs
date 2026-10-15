using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Subscription : AuditableEntity, IHouseholdScoped
{
    public Guid HouseholdId { get; set; }

    public Household? Household { get; set; }

    public string Plan { get; set; } = "free";

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Trial;

    public string? Provider { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
