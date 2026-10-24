using Domain.Common;

namespace Domain.Entities;

public class Invitation : AuditableEntity, IHouseholdScoped
{
    public Guid HouseholdId { get; set; }

    public Household? Household { get; set; }

    public string Code { get; set; } = string.Empty;

    public Guid CreatedByUserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public Guid? UsedByUserId { get; set; }

    public bool IsAvailable(DateTime now) => UsedAt is null && ExpiresAt > now;
}
