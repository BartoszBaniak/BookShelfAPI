using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Common;
using BookShelfAPI.Domain.Repositories;

namespace BookShelfAPI.Application.Books.GetAll;

public class GetBooksHandler(IBookRepository bookRepository)
    : IQueryHandler<GetBooksQuery, PagedResult<BookDto>>
{
    public async Task<PagedResult<BookDto>> HandleAsync(
        GetBooksQuery query,
        CancellationToken cancellationToken = default)
    {
        var skip = (query.Page - 1) * query.PageSize;

        var books = await bookRepository.GetPagedAsync(skip, query.PageSize, query.Status, cancellationToken);
        var total = await bookRepository.CountAsync(query.Status, cancellationToken);

        var items = books.Select(BookDto.FromDomain).ToList();

        return new PagedResult<BookDto>(items, query.Page, query.PageSize, total);
    }
}
