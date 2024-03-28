using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Applander.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlServerConnectionString = configuration.GetConnectionString("ApplendarDb");
        
        if (string.IsNullOrEmpty(sqlServerConnectionString))
            throw new Exception("Missing ApplendarDb environment variable.");
        
        try
        {
            services.AddDbContext<ApplendarDbContext>(options => options.UseNpgsql(sqlServerConnectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Thread.Sleep(5000);
        }

        return services;

    }
}