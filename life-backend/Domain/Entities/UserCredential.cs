using Domain.Common;

namespace Domain.Entities;

public class UserCredential : AuditableEntity
{
    public Guid UserId { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
