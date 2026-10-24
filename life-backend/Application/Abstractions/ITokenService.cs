using Domain.Entities;

namespace Application.Abstractions;

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);
}

public record AccessToken(string Value, DateTime ExpiresAt);
