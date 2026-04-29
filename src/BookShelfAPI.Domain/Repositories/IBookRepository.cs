using BookShelfAPI.Domain.Entities;

namespace BookShelfAPI.Domain.Repositories;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
}
