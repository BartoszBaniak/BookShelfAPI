using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Repositories;

namespace BookShelfAPI.Application.Books.GetById;

public class GetBookByIdHandler(IBookRepository bookRepository)
    : IQueryHandler<GetBookByIdQuery, BookDto>
{
    public async Task<BookDto> HandleAsync(GetBookByIdQuery query, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(query.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException($"Book with id '{query.Id}' was not found.");

        return BookDto.FromDomain(book);
    }
}
