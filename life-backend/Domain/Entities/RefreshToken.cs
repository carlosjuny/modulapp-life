using Domain.Common;

namespace Domain.Entities;

public class RefreshToken : AuditableEntity
{
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;
}
