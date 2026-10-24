using Domain.Entities;

namespace Application.Abstractions;

public interface IAuthStore
{
    Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken);

    Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken);

    Task RevokeActiveRefreshTokensAsync(Guid userId, DateTime revokedAt, CancellationToken cancellationToken);

    void Add<TEntity>(TEntity entity) where TEntity : class;

    Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>Guarda y devuelve false si otro proceso modificó el mismo refresh token (rotación concurrente).</summary>
    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken);
}
