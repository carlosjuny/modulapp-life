using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Membership : AuditableEntity, IHouseholdScoped
{
    public Guid UserId { get; set; }

    public User? User { get; set; }

    public Guid HouseholdId { get; set; }

    public Household? Household { get; set; }

    public MembershipRole Role { get; set; } = MembershipRole.Member;

    public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
}
