using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Repositories;
using BookShelfAPI.Domain.Services;

namespace BookShelfAPI.Application.Books.Update;

public class UpdateBookHandler(
    IBookRepository bookRepository,
    IBookIsbnUniquenessChecker isbnUniquenessChecker)
    : ICommandHandler<UpdateBookCommand>
{
    public async Task HandleAsync(UpdateBookCommand command, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException($"Book with id '{command.Id}' was not found.");

        await book.UpdateCatalogAsync(
            command.Title,
            command.Author,
            command.Isbn,
            command.PublicationYear,
            command.Description,
            isbnUniquenessChecker,
            cancellationToken);

        await bookRepository.UpdateAsync(book, cancellationToken);
    }
}
