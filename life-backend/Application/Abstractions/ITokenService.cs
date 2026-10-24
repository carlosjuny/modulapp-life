using Domain.Entities;

namespace Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);

    NewRefreshToken CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}

public record AccessToken(string Value, DateTime ExpiresAt);

public record NewRefreshToken(string Value, string Hash, DateTime ExpiresAt);
