using Microsoft.AspNetCore.Authorization;

namespace API.Authorization;

public class HouseholdAccessRequirement(bool adminOnly) : IAuthorizationRequirement
{
    public const string MemberPolicy = "HouseholdMember";
    public const string AdminPolicy = "HouseholdAdmin";

    public bool AdminOnly { get; } = adminOnly;
}
