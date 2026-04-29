using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Application.Repositories;
using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Services;

namespace BookShelfAPI.Application.Books.Create;

public class CreateBookHandler(
    IBookRepository bookRepository,
    IBookIsbnUniquenessChecker isbnUniquenessChecker)
    : ICommandHandler<CreateBookCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateBookCommand command, CancellationToken cancellationToken = default)
    {
        var book = await Book.CreateAsync(
            command.Title,
            command.Author,
            isbnUniquenessChecker,
            command.Isbn,
            command.PublicationYear,
            command.Description,
            command.Status,
            command.Rating,
            command.StartedReadingAt,
            command.FinishedReadingAt,
            cancellationToken);

        await bookRepository.AddAsync(book, cancellationToken);

        return book.Id;
    }
}
