using Application.Abstractions;
using Infrastructure.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.Configure<SocialAuthOptions>(configuration.GetSection(SocialAuthOptions.SectionName));

        services.AddSingleton(TimeProvider.System);

        services.AddSingleton<Application.Abstractions.IPasswordHasher, AspNetPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<ISocialSigningKeyProvider, OidcSocialSigningKeyProvider>();
        services.AddSingleton<IExternalTokenValidator, SocialTokenValidator>();

        return services;
    }
}
