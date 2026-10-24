using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AuthStore(ApplicationDbContext db) : IAuthStore
{
    public Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
        => db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken)
        => db.UserCredentials.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public void Add<TEntity>(TEntity entity) where TEntity : class => db.Add(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
