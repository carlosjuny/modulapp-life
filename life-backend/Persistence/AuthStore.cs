using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AuthStore(ApplicationDbContext db) : IAuthStore
{
    public Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
        => db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        => db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    public Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken)
        => db.UserCredentials.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken)
        => db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

    public async Task RevokeActiveRefreshTokensAsync(Guid userId, DateTime revokedAt, CancellationToken cancellationToken)
    {
        var active = await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in active)
        {
            token.RevokedAt = revokedAt;
        }
    }

    public void Add<TEntity>(TEntity entity) where TEntity : class => db.Add(entity);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }
}
