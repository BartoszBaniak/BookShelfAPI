using BookShelfAPI.Application.Repositories;
using BookShelfAPI.Domain.Entities;

namespace BookShelfAPI.Infrastructure.Persistence.Repositories;

public class BookRepository(BookDbContext db) : IBookRepository
{
    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await db.Books.AddAsync(book, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
