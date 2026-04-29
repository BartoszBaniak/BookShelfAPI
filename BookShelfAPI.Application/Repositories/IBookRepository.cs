using BookShelfAPI.Domain.Entities;

namespace BookShelfAPI.Application.Repositories;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
}
