using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Repositories;

namespace BookShelfAPI.Application.Books.Delete;

public class DeleteBookHandler(IBookRepository bookRepository)
    : ICommandHandler<DeleteBookCommand>
{
    public async Task HandleAsync(DeleteBookCommand command, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException($"Book with id '{command.Id}' was not found.");

        await bookRepository.DeleteAsync(book, cancellationToken);
    }
}
