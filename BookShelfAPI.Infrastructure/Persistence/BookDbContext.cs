using BookShelfAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookShelfAPI.Infrastructure.Persistence;

public class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
