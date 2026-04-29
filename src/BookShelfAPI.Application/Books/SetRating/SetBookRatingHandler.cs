using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Domain.Exceptions;
using BookShelfAPI.Domain.Repositories;

namespace BookShelfAPI.Application.Books.SetRating;

public class SetBookRatingHandler(IBookRepository bookRepository)
    : ICommandHandler<SetBookRatingCommand>
{
    public async Task HandleAsync(SetBookRatingCommand command, CancellationToken cancellationToken = default)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException($"Book with id '{command.Id}' was not found.");

        book.SetRating(command.Rating);

        await bookRepository.UpdateAsync(book, cancellationToken);
    }
}
