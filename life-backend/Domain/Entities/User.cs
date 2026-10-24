using Domain.Common;

namespace Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }

    public Guid? ActiveHouseholdId { get; set; }

    public ICollection<Membership> Memberships { get; set; } = [];
}
