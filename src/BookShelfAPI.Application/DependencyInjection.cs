using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Books;
using BookShelfAPI.Application.Books.ChangeStatus;
using BookShelfAPI.Application.Books.Create;
using BookShelfAPI.Application.Books.Delete;
using BookShelfAPI.Application.Books.GetAll;
using BookShelfAPI.Application.Books.GetById;
using BookShelfAPI.Application.Books.SetRating;
using BookShelfAPI.Application.Books.Statistics;
using BookShelfAPI.Application.Books.Update;
using BookShelfAPI.Application.Common;
using BookShelfAPI.Domain.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelfAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateBookCommand, Guid>, CreateBookHandler>();
        services.AddScoped<ICommandHandler<DeleteBookCommand>, DeleteBookHandler>();
        services.AddScoped<ICommandHandler<UpdateBookCommand>, UpdateBookHandler>();
        services.AddScoped<ICommandHandler<ChangeBookStatusCommand>, ChangeBookStatusHandler>();
        services.AddScoped<ICommandHandler<SetBookRatingCommand>, SetBookRatingHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookDto>, GetBookByIdHandler>();
        services.AddScoped<IQueryHandler<GetBooksQuery, PagedResult<BookDto>>, GetBooksHandler>();
        services.AddScoped<IQueryHandler<GetStatisticsQuery, BookStatistics>, GetStatisticsHandler>();
        return services;
    }
}
