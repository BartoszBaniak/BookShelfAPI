using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Enums;
using BookShelfAPI.Domain.Repositories;
using BookShelfAPI.Domain.Services;

namespace BookShelfAPI.Application.Common;

public static class BookSeeder
{
    public static async Task SeedAsync(
        IBookRepository repository,
        IBookIsbnUniquenessChecker isbnChecker,
        CancellationToken cancellationToken = default)
    {
        var existing = await repository.CountAsync(status: null, cancellationToken);
        if (existing > 0)
            return;

        var seeds = new[]
        {
            new SeedBook("Clean Code", "Robert C. Martin", "9780132350884", ReadingStatus.Finished, 5),
            new SeedBook("Clean Architecture", "Robert C. Martin", "9780134494166", ReadingStatus.Finished, 4),
            new SeedBook("The Clean Coder", "Robert C. Martin", "9780137081073", ReadingStatus.Reading, null),
            new SeedBook("Refactoring", "Martin Fowler", "9780134757599", ReadingStatus.Finished, 5),
            new SeedBook("Patterns of Enterprise Application Architecture", "Martin Fowler", "9780321127426", ReadingStatus.Unread, null),
            new SeedBook("Domain-Driven Design", "Eric Evans", "9780321125217", ReadingStatus.Reading, null),
            new SeedBook("Working Effectively with Legacy Code", "Michael Feathers", "9780131177055", ReadingStatus.Unread, null),
            new SeedBook("The Pragmatic Programmer", "Andrew Hunt", "9780135957059", ReadingStatus.Finished, 4),
        };

        foreach (var seed in seeds)
        {
            var book = await Book.CreateAsync(
                seed.Title,
                seed.Author,
                isbnChecker,
                isbn: seed.Isbn,
                status: seed.Status,
                rating: seed.Rating,
                cancellationToken: cancellationToken);

            await repository.AddAsync(book, cancellationToken);
        }
    }

    private record SeedBook(string Title, string Author, string Isbn, ReadingStatus Status, int? Rating);
}
