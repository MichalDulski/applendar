using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Applander.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlServerConnectionString = configuration.GetConnectionString("SQL_CONNECTION_STRING");

        if (!string.IsNullOrEmpty(sqlServerConnectionString))
        {
            services.AddDbContext<ApplendarDbContext>(options => options.UseSqlServer(sqlServerConnectionString));

            return services;
        }

        throw new Exception("Missing SQL_CONNECTION_STRING environment variable.");
    }
}