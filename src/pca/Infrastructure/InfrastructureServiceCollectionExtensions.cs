using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pca.Application.Common.Interfaces;
using pca.Infrastructure.Common;
using pca.Infrastructure.Persistence;

namespace pca.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {
        services
           .AddOptions<ConnectionStringsOptions>()
           .Bind(configuration.GetSection("ConnectionStrings"))
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.DatabaseConnection),
               "Database connection string is required.")
           .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionString = GetDbConnectionString(serviceProvider);

            options.UseNpgsql(
                connectionString,
                npgsql => npgsql
                    .MigrationsAssembly(
                        typeof(ApplicationDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null));

            if (webHostEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });

        services
            .AddHealthChecks() // app.MapHealthChecks("/health");
            .AddNpgSql(
                GetDbConnectionString(
                    services.BuildServiceProvider()));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static string GetDbConnectionString(IServiceProvider provider)
    {
        return provider
            .GetRequiredService<IOptions<ConnectionStringsOptions>>()
            .Value
            .DatabaseConnection;
    }
}
