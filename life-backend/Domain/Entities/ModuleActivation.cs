using Domain.Common;

namespace Domain.Entities;

public class ModuleActivation : AuditableEntity, IHouseholdScoped
{
    public Guid HouseholdId { get; set; }

    public Household? Household { get; set; }

    public string ModuleKey { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime? ActivatedAt { get; set; }
}
