using BookShelfAPI.Domain.Queries;
using BookShelfAPI.Domain.Repositories;
using BookShelfAPI.Domain.Services;
using BookShelfAPI.Infrastructure.Persistence;
using BookShelfAPI.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelfAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BookDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookIsbnUniquenessChecker, BookIsbnUniquenessChecker>();
        services.AddScoped<IBookStatisticsReader, BookStatisticsReader>();
        return services;
    }
}
