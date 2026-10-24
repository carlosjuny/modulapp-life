namespace Application.Households;

public enum HouseholdStatus
{
    Success,
    ValidationFailed,
    Forbidden,
    NotFound,
    Conflict,
    InvalidInvitation
}

public record HouseholdResult<T>(HouseholdStatus Status, T? Value = default, string? Error = null)
{
    public bool IsSuccess => Status == HouseholdStatus.Success;

    public static HouseholdResult<T> Success(T value) => new(HouseholdStatus.Success, value);

    public static HouseholdResult<T> Fail(HouseholdStatus status, string error) => new(status, default, error);
}

public record HouseholdSummary(Guid Id, string Name, string Role, bool IsActive);

public record MemberSummary(Guid UserId, string Name, string Email, string Role, DateTime JoinedAt);

public record ModuleSummary(string ModuleKey, bool IsActive);

public record PlanSummary(Guid HouseholdId, string Plan);

public record InvitationSummary(Guid HouseholdId, string Code, DateTime ExpiresAt);

public record Unit
{
    public static readonly Unit Value = new();
}
