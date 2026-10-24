using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IEventLogStore, EventLogStore>();
        services.AddScoped<IAuthStore, AuthStore>();
        services.AddScoped<IHouseholdStore, HouseholdStore>();

        return services;
    }
}
