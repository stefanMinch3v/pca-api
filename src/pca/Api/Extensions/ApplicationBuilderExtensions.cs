using pca.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace pca.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using (var services = app.ApplicationServices.CreateScope())
        {
            var dbContext = services.ServiceProvider.GetService<ApplicationDbContext>()!;
            dbContext.Database.Migrate();
        }

        return app;
    }
}