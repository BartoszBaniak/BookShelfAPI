namespace BookShelfAPI.Domain.Queries;

public interface IBookStatisticsReader
{
    Task<BookStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
