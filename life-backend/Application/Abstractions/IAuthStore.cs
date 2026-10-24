using Domain.Entities;

namespace Application.Abstractions;

public interface IAuthStore
{
    Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken);

    Task<ExternalLogin?> FindExternalLoginAsync(string provider, string subject, CancellationToken cancellationToken);

    Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken);

    Task RevokeActiveRefreshTokensAsync(Guid userId, DateTime revokedAt, CancellationToken cancellationToken);

    void Add<TEntity>(TEntity entity) where TEntity : class;

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken);
}
