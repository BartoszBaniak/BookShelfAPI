using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Repositories;

namespace BookShelfAPI.Application.Books.ChangeStatus;

public class ChangeBookStatusHandler(IBookRepository bookRepository)
    : ICommandHandler<ChangeBookStatusCommand>
{
    public async Task HandleAsync(ChangeBookStatusCommand command, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException($"Book with id '{command.Id}' was not found.");

        book.SetStatus(command.Status);

        await bookRepository.UpdateAsync(book, cancellationToken);
    }
}
