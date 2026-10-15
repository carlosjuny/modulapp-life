using Domain.Common;

namespace Domain.Entities;

public class Household : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Plan { get; set; } = "free";

    public Guid CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }

    public string TimeZone { get; set; } = "UTC";

    public ICollection<Membership> Memberships { get; set; } = [];

    public ICollection<ModuleActivation> ModuleActivations { get; set; } = [];

    public Subscription? Subscription { get; set; }
}
