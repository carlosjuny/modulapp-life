using Domain.Entities;

namespace Application.Abstractions;

public interface IAuthStore
{
    Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken);

    void Add<TEntity>(TEntity entity) where TEntity : class;

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
