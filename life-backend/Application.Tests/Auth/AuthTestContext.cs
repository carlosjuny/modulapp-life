using Application.Abstractions;
using Application.Auth;
using Domain.Entities;

namespace Application.Tests.Auth;

public class AuthTestContext
{
    public FakeAuthStore Store { get; } = new();

    public AuthService Service { get; }

    public AuthTestContext()
    {
        Service = new AuthService(Store, new FakePasswordHasher(), new FakeTokenService());
    }
}

public class FakeAuthStore : IAuthStore
{
    public List<User> Users { get; } = [];

    public List<UserCredential> Credentials { get; } = [];

    public Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
        => Task.FromResult(Users.FirstOrDefault(u => u.Email == email));

    public Task<UserCredential?> FindCredentialAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(Credentials.FirstOrDefault(c => c.UserId == userId));

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
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hash:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"hash:{password}";
}

public class FakeTokenService : ITokenService
{
    public AccessToken CreateAccessToken(User user) => new($"access-{user.Id}", DateTime.UtcNow.AddMinutes(15));
}
