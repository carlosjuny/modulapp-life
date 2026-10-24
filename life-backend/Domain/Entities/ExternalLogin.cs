using Domain.Common;

namespace Domain.Entities;

public class ExternalLogin : AuditableEntity
{
    public Guid UserId { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
