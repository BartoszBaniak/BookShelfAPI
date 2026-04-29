using BookShelfAPI.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace BookShelfAPI.Infrastructure.Persistence;

public class BookIsbnUniquenessChecker(BookDbContext db) : IBookIsbnUniquenessChecker
{
    public Task<bool> IsUniqueAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return db.Books.AllAsync(b => b.Isbn != isbn, cancellationToken);
    }
}
