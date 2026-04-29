using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Enums;
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

    public async Task<IReadOnlyList<Book>> GetPagedAsync(
        int skip,
        int take,
        ReadingStatus? status,
        CancellationToken cancellationToken = default)
    {
        return await ApplyStatusFilter(db.Books, status)
            .OrderBy(b => b.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ReadingStatus? status, CancellationToken cancellationToken = default)
    {
        return await ApplyStatusFilter(db.Books, status).CountAsync(cancellationToken);
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Book> ApplyStatusFilter(IQueryable<Book> source, ReadingStatus? status)
    {
        return status.HasValue ? source.Where(b => b.Status == status.Value) : source;
    }
}
