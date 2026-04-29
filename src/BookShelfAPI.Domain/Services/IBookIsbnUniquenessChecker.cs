namespace BookShelfAPI.Domain.Services;

public interface IBookIsbnUniquenessChecker
{
    Task<bool> IsUniqueAsync(string isbn, CancellationToken cancellationToken = default);
}
