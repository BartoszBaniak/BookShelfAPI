using BookShelfAPI.Application.Abstractions;
using BookShelfAPI.Domain.Queries;

namespace BookShelfAPI.Application.Books.Statistics;

public class GetStatisticsHandler(IBookStatisticsReader statisticsReader)
    : IQueryHandler<GetStatisticsQuery, BookStatistics>
{
    public Task<BookStatistics> HandleAsync(GetStatisticsQuery query, CancellationToken cancellationToken = default)
    {
        return statisticsReader.GetStatisticsAsync(cancellationToken);
    }
}
