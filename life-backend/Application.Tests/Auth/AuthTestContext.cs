using Application.Abstractions;
using Application.Auth;
using Domain.Entities;

namespace Application.Tests.Auth;

public class AuthTestContext
{
    public FakeAuthStore Store { get; } = new();

    public FakeClock Clock { get; } = new();

    public AuthService Service { get; }

    public AuthTestContext()
    {
        Service = new AuthService(Store, new FakePasswordHasher(), new FakeTokenService(Clock), Clock);
    }
}

public class FakeAuthStore : IAuthStore
{
    public List<User> Users { get; } = [];

    public List<UserCredential> Credentials { get; } = [];

    public List<RefreshToken> RefreshTokens { get; } = [];

    public bool ConcurrentRotationDetected { get; set; }

    public Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
        => Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

    public Task<User?> FindUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(Users.FirstOrDefault(u => u.Id == userId));

    public Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(Credentials.FirstOrDefault(c => c.UserId == userId));

    public Task<RefreshToken?> FindRefreshTokenAsync(string tokenHash, CancellationToken cancellationToken)
        => Task.FromResult(RefreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash));

    public Task RevokeActiveRefreshTokensAsync(Guid userId, DateTime revokedAt, CancellationToken cancellationToken)
    {
        foreach (var token in RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt is null))
        {
            token.RevokedAt = revokedAt;
        }

        return Task.CompletedTask;
    }

    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        switch (entity)
        {
            case User user:
                Users.Add(user);
                break;
            case UserCredential credential:
                credential.UserId = credential.User?.Id ?? credential.UserId;
                Credentials.Add(credential);
                break;
            case RefreshToken refreshToken:
                refreshToken.UserId = refreshToken.User?.Id ?? refreshToken.UserId;
                RefreshTokens.Add(refreshToken);
                break;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(!ConcurrentRotationDetected);
}

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hash:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"hash:{password}";
}

public class FakeTokenService(TimeProvider clock) : ITokenService
{
    private int _refreshCounter;

    public AccessToken CreateAccessToken(User user) => new($"access-{user.Id}", clock.GetUtcNow().UtcDateTime.AddMinutes(15));

    public NewRefreshToken CreateRefreshToken()
    {
        var value = $"refresh-{++_refreshCounter}";

        return new NewRefreshToken(value, HashRefreshToken(value), clock.GetUtcNow().UtcDateTime.AddDays(30));
    }

    public string HashRefreshToken(string refreshToken) => $"hash:{refreshToken}";
}
