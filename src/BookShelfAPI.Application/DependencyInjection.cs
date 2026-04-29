using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Application.Books.GetById;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelfAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateBookCommand, Guid>, CreateBookHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookDto>, GetBookByIdHandler>();
        return services;
    }
}
