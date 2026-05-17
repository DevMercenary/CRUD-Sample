using CrudSample.Core.Application;
using CrudSample.Core.Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrudSample.Core.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Wires Domain → Application → Infrastructure into the DI container.
    /// The Api project just calls this once and gets the full stack.
    /// </summary>
    public static IServiceCollection AddCrudSampleCore(
        this IServiceCollection services,
        string sqliteConnectionString)
    {
        services.AddDbContext<AppDbContext>(opts => opts.UseSqlite(sqliteConnectionString));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        return services;
    }
}
