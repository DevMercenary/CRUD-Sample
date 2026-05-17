using CrudSample.Core.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CrudSample.Tests;

/// <summary>
/// Hosts the real API pipeline with EF Core swapped to an in-memory provider
/// so tests stay hermetic. Each instance uses a unique database name so tests
/// run in isolation without xUnit collection ceremony.
/// </summary>
public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString("N");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            // Strip every registration that ties the context to a real provider,
            // then re-register with the in-memory provider scoped to this run.
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext))
                .ToList();
            foreach (var descriptor in toRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(opts =>
                opts.UseInMemoryDatabase(_databaseName));
        });
    }
}
