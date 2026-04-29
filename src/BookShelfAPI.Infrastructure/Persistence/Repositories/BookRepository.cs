using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BookShelfAPI.Infrastructure.Persistence.Repositories;

public class BookRepository(BookDbContext db) : IBookRepository
{
    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await db.Books.AddAsync(book, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await db.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}
