using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books.Create;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelfAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateBookCommand, Guid>, CreateBookHandler>();
        return services;
    }
}
