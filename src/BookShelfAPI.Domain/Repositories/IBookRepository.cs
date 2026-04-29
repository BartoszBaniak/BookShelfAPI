using BookShelfAPI.Domain.Entities;
using BookShelfAPI.Domain.Enums;

namespace BookShelfAPI.Domain.Repositories;

public interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken = default);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Book>> GetPagedAsync(
        int skip,
        int take,
        ReadingStatus? status,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(ReadingStatus? status, CancellationToken cancellationToken = default);
}
