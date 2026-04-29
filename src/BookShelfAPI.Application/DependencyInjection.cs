using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Application.Books.Delete;
using BookShelfAPI.Application.Books.GetAll;
using BookShelfAPI.Application.Books.GetById;
using BookShelfAPI.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelfAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateBookCommand, Guid>, CreateBookHandler>();
        services.AddScoped<ICommandHandler<DeleteBookCommand>, DeleteBookHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookDto>, GetBookByIdHandler>();
        services.AddScoped<IQueryHandler<GetBooksQuery, PagedResult<BookDto>>, GetBooksHandler>();
        return services;
    }
}
